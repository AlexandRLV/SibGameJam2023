using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DI;
using LocalMessages;
using NetFrame;
using NetFrame.Enums;
using NetFrame.Utils;
using Networking.LocalMessages;
using Newtonsoft.Json;
using Startup;
using UnityEngine;
using WebSocketSharp;

namespace Networking
{
    public class WebSocketGameClient : IGameClient
    {
        private ClientParameters _parameters;
        private LocalMessageBroker _messageBroker;
        private GameClientData _data;
        private MonoUpdater _monoUpdater;

        private WebSocket _socket;

        private ConcurrentQueue<IDataframeWrapper> _incomingDataframes;

        [Construct]
        public WebSocketGameClient(ClientParameters parameters, LocalMessageBroker messageBroker, GameClientData data, MonoUpdater monoUpdater)
        {
            _parameters = parameters;
            _messageBroker = messageBroker;
            _data = data;
            _monoUpdater = monoUpdater;
            _incomingDataframes = new ConcurrentQueue<IDataframeWrapper>();
        }

        public void Connect()
        {
            Debug.Log("Connecting!");
            _monoUpdater.OnUpdate += Update;
            _socket = new WebSocket($"ws://{_parameters.Ip}:{_parameters.webSocketPort.ToString()}");
            
            Debug.Log("Created web socket");
            _socket.OnOpen += OnOpen;
            _socket.OnError += OnError;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            
            Debug.Log("Calling connect!");
            _socket.Connect();
            Debug.Log("Socket connect test");
        }

        public void Disconnect()
        {
            _socket.Close();
            _monoUpdater.OnUpdate -= Update;
            _incomingDataframes.Clear();
        }

        private void Update()
        {
            while (_incomingDataframes.TryDequeue(out var wrapper))
            {
                wrapper.Trigger(_messageBroker);
            }
        }

        public void Send<T>(ref T dataframe) where T : struct, INetworkDataframe
        {
            var type = typeof(T);
            var wrapper = new DataframeWrapper<T>
            {
                dataframe = dataframe
            };
            string contents = JsonConvert.SerializeObject(wrapper);
            var container = new DataframeContainer
            {
                contents = contents,
                dataframeType = type.Name
            };
            string json = JsonConvert.SerializeObject(container);
            _socket.Send(json);
        }
        
        private void OnOpen(object sender, EventArgs e)
        {
            Debug.Log("Websocket open!");
            _data.IsConnected = true;
            var message = new ConnectedMessage();
            _messageBroker.Trigger(ref message);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.Log($"Websocket error! {e.Exception}");
            _data.IsConnected = false;
            var message = new ConnectionFailedMessage
            {
                reason = ReasonServerConnectionFailed.ConnectionLost
            };
            _messageBroker.Trigger(ref message);
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            Debug.Log("Websocked close!");
            _data.IsConnected = false;
            var message = new ConnectionFailedMessage
            {
                reason = ReasonServerConnectionFailed.ConnectionLost
            };
            _messageBroker.Trigger(ref message);
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            Debug.Log($"Received new message: {e.Data}");
            var container = JsonConvert.DeserializeObject<DataframeContainer>(e.Data);
            Debug.Log($"Deserialized container, type: {container.dataframeType}");
            var dataframe = NetFrameDataframeCollection.GetByKey(container.dataframeType);
            var dataframeType = dataframe.GetType();
            Debug.Log($"Found dataframe type: {dataframeType.Name}");
            var wrapperType = typeof(DataframeWrapper<>).MakeGenericType(dataframeType);
            Debug.Log($"Got generic wrapper: {wrapperType.Name}");
            var wrapper = (IDataframeWrapper)JsonConvert.DeserializeObject(container.contents, wrapperType);
            _incomingDataframes.Enqueue(wrapper);
        }
    }
}