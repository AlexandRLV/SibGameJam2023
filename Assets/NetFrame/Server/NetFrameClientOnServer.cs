using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using NetFrame.Constants;
using NetFrame.Utils;
using NetFrame.WriteAndRead;

namespace NetFrame.Server
{
    public class NetFrameClientOnServer
    {
        private readonly int _id;
        private readonly TcpClient _tcpSocket;
        private readonly NetworkStream _networkStream;
        private readonly NetFrameByteConverter _byteConverter;
        private readonly ConcurrentDictionary<Type, Delegate> _handlers;
        
        private readonly byte[] _receiveBuffer;
        private byte[] _receiveBufferOversize;
        
        private readonly int _receiveBufferSize; 

        private NetFrameReader _reader;

        private bool _isReadProcess;
        private bool _isOversizeReceiveBuffer;
        
        public TcpClient TcpSocket => _tcpSocket;

        public NetFrameClientOnServer(int id, TcpClient tcpSocket, ConcurrentDictionary<Type, Delegate> handlers, 
            int bufferSize)
        {
            _id = id;
            _tcpSocket = tcpSocket;
            _handlers = handlers;
            _networkStream = tcpSocket.GetStream();
            _byteConverter = new NetFrameByteConverter();
            _reader = new NetFrameReader(new byte[bufferSize]);
            _receiveBufferSize = bufferSize;
            _receiveBuffer = new byte[_receiveBufferSize];
        }

        public void CheckAvailableBytes()
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

                    var datagram = NetFrameDatagramCollection.GetDatagramByKey(headerDatagram);
                    var targetType = datagram.GetType();
                    
                    _reader.SetBuffer(contentSegment);
                    datagram.Read(_reader);
                    
                    if (_handlers.TryGetValue(targetType, out var handler))
                    {
                        MainThread.Run(() =>
                        {
                            handler.DynamicInvoke(datagram, _id);
                        });
                    }
                } 
                while (readBytesCompleteCount < allBytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error receive TCP Client {e.Message}");
                MainThread.Run(Disconnect);
            }
        }

        public void Disconnect()
        {
            _tcpSocket.Close();
        }
    }
}