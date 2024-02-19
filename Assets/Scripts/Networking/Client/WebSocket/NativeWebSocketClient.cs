using System;
using System.Collections.Concurrent;
using System.Text;
using Common.DI;
using LocalMessages;
using NativeWebSocket;
using NetFrame;
using NetFrame.Enums;
using NetFrame.Utils;
using Networking.LocalMessages;
using Newtonsoft.Json;
using Startup;
using UnityEngine;

namespace Networking.Client.WebSocket
{
    public class NativeWebSocketClient : IGameClient
    {
        private ClientParameters _parameters;
        private LocalMessageBroker _messageBroker;
        private GameClientData _data;
        private MonoUpdater _monoUpdater;
        
        private NativeWebSocket.WebSocket _webSocket;
        
        private ConcurrentQueue<IDataframeWrapper> _incomingDataframes;

        [Construct]
        public NativeWebSocketClient(ClientParameters parameters, LocalMessageBroker messageBroker, GameClientData data, MonoUpdater monoUpdater)
        {
            _parameters = parameters;
            _messageBroker = messageBroker;
            _data = data;
            _monoUpdater = monoUpdater;

            _incomingDataframes = new ConcurrentQueue<IDataframeWrapper>();
        }

        public async void Connect()
        {
            Debug.Log("Creating web socket");
            _webSocket = new NativeWebSocket.WebSocket($"ws://{_parameters.Ip}:{_parameters.webSocketPort.ToString()}");
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnClose += OnClose;
            _webSocket.OnError += OnError;
            _webSocket.OnMessage += OnMessage;
            
            Debug.Log("Connecting web socket");
            _monoUpdater.OnUpdate += OnUpdate;
            await _webSocket.Connect();
        }

        public void Disconnect()
        {
            if (_webSocket == null)
                return;
            
            _webSocket.Close();
            _webSocket.OnOpen -= OnOpen;
            _webSocket.OnClose -= OnClose;
            _webSocket.OnError -= OnError;
            _webSocket.OnMessage -= OnMessage;
            _monoUpdater.OnUpdate -= OnUpdate;
            _incomingDataframes.Clear();
            _webSocket = null;
        }

        public void Send<T>(ref T dataframe) where T : struct, INetworkDataframe
        {
            if (_webSocket == null)
                return;
            
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
            _webSocket.SendText(json);
        }

        private void OnUpdate()
        {
            while (_incomingDataframes.TryDequeue(out var wrapper))
            {
                Debug.Log($"Triggering dataframe {wrapper.GetType().Name}");
                wrapper.Trigger(_messageBroker);
            }
            
#if !UNITY_WEBGL || UNITY_EDITOR
            if (_data.IsConnected) _webSocket.DispatchMessageQueue();
#endif
        }

        private void OnOpen()
        {
            Debug.Log("Connection open!");
            _data.IsConnected = true;
            var message = new ConnectedMessage();
            _messageBroker.Trigger(ref message);
        }

        private void OnClose(WebSocketCloseCode closecode)
        {
            Debug.Log($"Connection close! {closecode}");
            _data.IsConnected = false;
            var message = new ConnectionFailedMessage
            {
                reason = ReasonServerConnectionFailed.ConnectionLost
            };
            _messageBroker.Trigger(ref message);
        }

        private void OnError(string errormsg)
        {
            Debug.Log($"Connection error! {errormsg}");
            _data.IsConnected = false;
            var message = new ConnectionFailedMessage
            {
                reason = ReasonServerConnectionFailed.ConnectionLost
            };
            _messageBroker.Trigger(ref message);
        }

        private void OnMessage(byte[] data)
        {
            Debug.Log("Received message!");
            string message = Encoding.Default.GetString(data);
            
            Debug.Log($"Received new message: {message}");
            var container = JsonConvert.DeserializeObject<DataframeContainer>(message);
            Debug.Log($"Deserialized container, type: {container.dataframeType}");
            var dataframe = NetFrameDataframeCollection.GetByKey(container.dataframeType);
            var dataframeType = dataframe.GetType();
            Debug.Log($"Found dataframe type: {dataframeType.Name}");
            var wrapperType = typeof(DataframeWrapper<>).MakeGenericType(dataframeType);
            Debug.Log($"Got generic wrapper: {wrapperType.FullName}");
            object deserialized = null;
            try
            {
                deserialized = JsonConvert.DeserializeObject(container.contents, wrapperType);
            }
            catch (Exception e)
            {
                Debug.LogError($"Cannot deserialize dataframe, exception: {e}");
            }
            Debug.Log($"Deserialized: {deserialized.ToString()} with type {deserialized.GetType().FullName}");
            if (deserialized is not IDataframeWrapper)
            {
                Debug.LogError("Desetialized object is wrong type!");
                return;
            }
            var wrapper = (IDataframeWrapper)deserialized;
            _incomingDataframes.Enqueue(wrapper);
        }
    }
}