using System.Net.WebSockets;
using System.Text;

namespace DjStreamer.Models.WebSockets
{
    public class ConnectionBase
    {
        protected TaskCompletionSource<object> _source { get; set; }
        protected WebSocket _socket { get; private set; } = null;

        System.Timers.Timer _timer;

        public ConnectionBase(TaskCompletionSource<object> source, WebSocket socket, bool isServer = false)
        {
            _source = source;
            _socket = socket;
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

                            RecieveData(bytes);
                        }
                    }
                    else //Assuming Heartbeat
                    {
                        //If message recieved, restart timer;
                        _timer.Stop();
                        _timer.Start();
                    }
                }
                catch {
                    //Disconnected
                    //EndConnection();
                }
            }
        }

        protected virtual void RecieveData(byte[] bytes) { 
        }
        
        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            EndConnection();
        }

        public void EndConnection()
        {
            _timer.Stop();

            try
            {
                _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            catch (Exception ex)
            {
            }

            _source.TrySetResult(null);
        }

        public async void SendData(byte[] bytes)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(bytes);

            await _socket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
