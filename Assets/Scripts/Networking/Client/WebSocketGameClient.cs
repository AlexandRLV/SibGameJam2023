using System;
using Common.DI;
using LocalMessages;
using NetFrame;
using NetFrame.Utils;
using Newtonsoft.Json;
using Startup;
using UnityEngine;
using WebSocketSharp;

namespace Networking
{
    public class WebSocketGameClient
    {
        public event Action OnConnectionOpen;
        public event Action OnConnectionError;
        
        private ClientParameters _parameters;
        private LocalMessageBroker _messageBroker;

        private WebSocket _socket;

        [Construct]
        public WebSocketGameClient(ClientParameters parameters, LocalMessageBroker messageBroker)
        {
            _parameters = parameters;
            _messageBroker = messageBroker;
        }

        public void Connect()
        {
            _socket = new WebSocket($"ws://{_parameters.Ip}:{_parameters.port.ToString()}");
            
            _socket.OnOpen += OnOpen;
            _socket.OnError += OnError;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            
            _socket.Connect();
            Debug.Log("Socket connect test");
        }

        public void Disconnect()
        {
            _socket.Close();
        }

        public void Send<T>(T dataframe) where T : struct, INetworkDataframe
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
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.Log($"Websocket error! {e.Message}");
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            Debug.Log("Websocked close!");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            Debug.Log($"Received new message: {e.Data}");
            var container = JsonConvert.DeserializeObject<DataframeContainer>(e.Data);
            Debug.Log($"Deserialized container, type: {container.dataframeType}");
            var dataframeType = NetFrameDataframeCollection.GetByKey(container.contents).GetType();
            Debug.Log($"Found dataframe type: {dataframeType.Name}");
            var wrapperType = typeof(DataframeWrapper<>).MakeGenericType(dataframeType);
            Debug.Log($"Got generic wrapper: {wrapperType.Name}");
            var wrapper = (IDataframeWrapper)JsonConvert.DeserializeObject(container.contents, wrapperType);
            Debug.Log($"Deserialized wrapper, triggering!");
            wrapper.Trigger(_messageBroker);
        }
    }
}