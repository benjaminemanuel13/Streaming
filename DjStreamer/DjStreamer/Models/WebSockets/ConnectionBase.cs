using DjStreamer.Business.Services;
using DjStreamer.Business.Services.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace DjStreamer.Models.WebSockets
{
    public class ConnectionBase
    {
        protected TaskCompletionSource<object> _source { get; set; }
        protected WebSocket _socket { get; private set; }

        System.Timers.Timer _timer;
        int counter = 0;

        protected FileStream stream = null;

        protected readonly ILoggingService _log;
        protected readonly IWebHostEnvironment _host;

        public ConnectionBase(IWebHostEnvironment host, TaskCompletionSource<object> source, WebSocket socket, bool isServer = false)
        {
            _host = host;
            _log = new LoggingService(_host.WebRootPath);

            _source = source;
            _socket = socket;

            StartListening();

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += _timer_Elapsed;

            //if(!isServer)
            //    _timer.Start();
        }

        private async void StartListening()
        {
            while (true)
            {
                try
                {
                    byte[] bytes = new byte[5];

                    await _socket.ReceiveAsync(bytes, CancellationToken.None);

                    string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    if (message == "Close")
                    {
                        EndConnection();
                    }
                    else if (message == "GotIt")
                    {
                        bytes = new byte[6];

                        await _socket.ReceiveAsync(bytes, CancellationToken.None);

                        message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                        int length = int.Parse(message);

                        if (length > 0)
                        {
                            bytes = new byte[length];

                            await _socket.ReceiveAsync(bytes, CancellationToken.None);

                            //string recieve = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                            //byte[] data = Convert.FromBase64String(recieve);

                            RecieveData(bytes);
                        }
                    }
                    else //Assuming Heartbeat
                    {
                        counter = 0;
                    }
                }
                catch (Exception ex) {
                    _log.Log("Server StartListening: " + ex.Message);

                    //Disconnected
                    EndConnection();

                    break;
                }
            }
        }

        protected virtual void RecieveData(byte[] bytes) { 
        }
        
        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (counter > 9)
                EndConnection();
            else
                counter++;
        }

        public void EndConnection()
        {
            _timer.Stop();

            try
            {
                if (_socket.State == WebSocketState.Open)
                {
                    _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _log.Log("Connection Exception: " + ex.Message);
            }

            //stream.Close();

            _source.TrySetResult(null);
        }

        public async Task SendData(byte[] bytes)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);

            await _socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
