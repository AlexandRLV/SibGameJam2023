using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetFrame.Constants;
using NetFrame.Enums;
using NetFrame.Utils;
using NetFrame.WriteAndRead;

namespace NetFrame.Client
{
    public class NetFrameClient
    {
        private readonly NetFrameByteConverter _byteConverter;
        private readonly ConcurrentDictionary<Type, Delegate> _handlers;
        private readonly NetFrameDatagramCollection _datagramCollection;
        
        private TcpClient _tcpSocket;
        private NetworkStream _networkStream;

        private NetFrameWriter _writer;
        private NetFrameReader _reader;
        
        private byte[] _receiveBuffer;
        private byte[] _receiveBufferOversize;

        private int _receiveBufferSize;
        private int _writeBufferSize;
        
        private bool _isReadProcess;
        private bool _isOversizeReceiveBuffer;

        public event Action<ReasonServerConnectionFailed> ConnectedFailed;
        public event Action ConnectionSuccessful;
        public event Action Disconnected;

        public NetFrameClient()
        {
            _handlers = new ConcurrentDictionary<Type, Delegate>();
            _byteConverter = new NetFrameByteConverter();
            _datagramCollection = new NetFrameDatagramCollection();
        }

        public void Connect(string host, int port, int receiveBufferSize = 4096, int writeBufferSize = 4096)
        {
            if (_tcpSocket != null && _tcpSocket.Connected)
            {
                ConnectedFailed?.Invoke(ReasonServerConnectionFailed.AlreadyConnected);
                return;
            }

            _tcpSocket = new TcpClient();

            _receiveBufferSize = receiveBufferSize;
            _writeBufferSize = writeBufferSize;
            _receiveBuffer = new byte[receiveBufferSize];

            _writer = new NetFrameWriter(_writeBufferSize);
            _reader = new NetFrameReader(new byte[_receiveBufferSize]);

            _tcpSocket.BeginConnect(host, port, BeginConnectCallback, _tcpSocket);
        }

        public void Run()
        {
            CheckDisconnectToServer();
            CheckAvailableBytes();
            MainThread.Pulse();
        }

        private void BeginConnectCallback(IAsyncResult result)
        {
            var tcpSocket = (TcpClient) result.AsyncState;

            if (!tcpSocket.Connected)
            {
                MainThread.Run(() =>
                {
                    ConnectedFailed?.Invoke(ReasonServerConnectionFailed.ImpossibleToConnect);
                });
                return;
            }

            _networkStream = tcpSocket.GetStream();
            
            MainThread.Run(() =>
            {
                ConnectionSuccessful?.Invoke();
            });
        }
        
        private void CheckAvailableBytes()
        {
            if (_networkStream != null && _networkStream.CanRead && _networkStream.DataAvailable && !_isReadProcess)
            {
                var availableBytes = _tcpSocket.Available;

                if (availableBytes > _receiveBufferSize)
                {
                    _receiveBufferOversize = new byte[availableBytes];
                    _reader = new NetFrameReader(new byte[availableBytes]);
                    _isOversizeReceiveBuffer = true;
                }
                else if (_isOversizeReceiveBuffer)
                {
                    _isOversizeReceiveBuffer = false;
                    _reader = new NetFrameReader(new byte[_receiveBufferSize]);
                }

                if (_isOversizeReceiveBuffer)
                {
                    _networkStream.BeginRead(_receiveBufferOversize, 0, availableBytes, BeginReadBytesCallback, null);
                }
                else
                {
                    _networkStream.BeginRead(_receiveBuffer, 0, _receiveBufferSize, BeginReadBytesCallback, null);
                }
                
                _isReadProcess = true;
            }
        }

        private void BeginReadBytesCallback(IAsyncResult result)
        {
            try
            {
                if (!_networkStream.CanRead)
                {
                    return;
                }
                
                var byteReadLength = _networkStream.EndRead(result);
                _isReadProcess = false;

                if (byteReadLength <= 0)
                {
                    return;
                }

                var allBytes = new byte[byteReadLength];

                Array.Copy( _isOversizeReceiveBuffer ? _receiveBufferOversize : _receiveBuffer, 
                    allBytes, byteReadLength);
                
                var readBytesCompleteCount = 0;

                do
                {
                    var packageSizeSegment = new ArraySegment<byte>(allBytes, readBytesCompleteCount,
                        NetFrameConstants.SizeByteCount);
                    var packageSize = _byteConverter.GetUIntFromByteArray(packageSizeSegment.ToArray());
                    var packageBytes = new ArraySegment<byte>(allBytes, readBytesCompleteCount, packageSize);

                    var tempIndex = 0;
                    for (var index = NetFrameConstants.SizeByteCount; index < packageSize; index++)
                    {
                        var b = packageBytes[index];

                        if (b == '\n')
                        {
                            tempIndex = index + 1;
                            break;
                        }
                    }

                    var headerSegment = new ArraySegment<byte>(packageBytes.ToArray(),
                        NetFrameConstants.SizeByteCount,
                        tempIndex - NetFrameConstants.SizeByteCount - 1);
                    var contentSegment =
                        new ArraySegment<byte>(packageBytes.ToArray(), tempIndex, packageSize - tempIndex);
                    var headerDatagram = Encoding.UTF8.GetString(headerSegment);

                    readBytesCompleteCount += packageSize;

                    var datagram = _datagramCollection.GetDatagramByKey(headerDatagram);
                    var targetType = datagram.GetType();
                    
                    _reader.SetBuffer(contentSegment);
                    datagram.Read(_reader);
                    
                    if (_handlers.TryGetValue(targetType, out var handler))
                    {
                        MainThread.Run(() =>
                        {
                            handler.DynamicInvoke(datagram);
                        });
                    }
                } 
                while (readBytesCompleteCount < allBytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receive TCP Client {e.Message}");
                //Debug.LogError($"Error receive TCP Client {e.Message}");
                
                MainThread.Run(Disconnect);
            }
        }

        public void Disconnect()
        {
            if (_tcpSocket != null && _tcpSocket.Connected)
            {
                _tcpSocket.Close();

                Disconnected?.Invoke();
            }
        }

        public void Send<T>(ref T datagram) where T : struct, INetFrameDatagram
        {
            _writer.Reset();
            datagram.Write(_writer);

            var separator = '\n';
            var headerDatagram = GetDatagramTypeName(datagram) + separator;

            var heaterDatagram = Encoding.UTF8.GetBytes(headerDatagram);
            var dataDatagram = _writer.ToArraySegment();
            var allData = heaterDatagram.Concat(dataDatagram).ToArray();

            var allPackageSize = (uint)allData.Length + NetFrameConstants.SizeByteCount;
            var sizeBytes = _byteConverter.GetByteArrayFromUInt(allPackageSize);
            var allPackage = sizeBytes.Concat(allData).ToArray();

            Task.Run(async () =>
            {
                await SendAsync(_networkStream, allPackage);
            });
        }
        
        private async Task SendAsync(NetworkStream networkStream, ArraySegment<byte> data)
        {
            await networkStream.WriteAsync(data);
        }

        public void Subscribe<T>(Action<T> handler) where T : struct, INetFrameDatagram
        {
            _handlers.AddOrUpdate(typeof(T), handler, (_, currentHandler) => (Action<T>)currentHandler + handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct, INetFrameDatagram
        {
            _handlers.TryRemove(typeof(T), out var currentHandler);
        }

        private string GetDatagramTypeName<T>(T datagram) where T : struct, INetFrameDatagram
        {
            return typeof(T).Name;
        }
        
        private void CheckDisconnectToServer()
        {
            if (_tcpSocket == null || !_tcpSocket.Connected)
            {
                return;
            }
            
            if (!_tcpSocket.Client.Poll(0, SelectMode.SelectRead))
            {
                return;
            }
            
            var buff = new byte[1];
            
            
            if (_tcpSocket.Client.Receive(buff, SocketFlags.Peek) != 0)
            {
                return;
            }
            
            _tcpSocket.Client.Disconnect(false);
            
            ConnectedFailed?.Invoke(ReasonServerConnectionFailed.ConnectionLost);
        }
    }
}