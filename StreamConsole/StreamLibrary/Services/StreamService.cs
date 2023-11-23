using StreamLibrary.EventArguments;
using StreamLibrary.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibrary.Services
{
    public enum StreamState
    {
        Stopped,
        Running
    }

    public class StreamService : IStreamService
    {
        public event EventHandler<ServerDisconnectedEventArgs> ServerDisconnected;

        ClientWebSocket socket = null;

        public StreamState State { get; set; } = StreamState.Stopped;

        FileStream stream = null;

        public async Task Connect()
        {
            socket = new ClientWebSocket();
#if DEBUG
            await socket.ConnectAsync(new Uri("wss://localhost:7130/ws/Server"), CancellationToken.None);
            //await socket.ConnectAsync(new Uri("ws://localhost:5130/ws/Server"), CancellationToken.None);
            //await socket.ConnectAsync(new Uri("wss://smile-stream.club/ws/Server"), CancellationToken.None);
            
#else
            await socket.ConnectAsync(new Uri("ws://192.168.0.15:5130/ws/Server"), CancellationToken.None);
            //await socket.ConnectAsync(new Uri("wss://smile-stream.club/ws/Server"), CancellationToken.None);
#endif
        }

        public async Task Disconnect()
        {
            try
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
            catch { }
        }

        public async Task Send(byte[] bytes)
        {
            string init = "GotIt";
            string length = bytes.Length.ToString("000000");

            var initbytes = Encoding.UTF8.GetBytes(init);
            var lengthbytes = Encoding.UTF8.GetBytes(length);

            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(initbytes, WebSocketMessageType.Text, true, CancellationToken.None);
                    
                    //string base64 = Convert.ToBase64String(bytes);
                    //byte[] send = Encoding.UTF8.GetBytes(base64);
                    length = bytes.Length.ToString("000000");
                    lengthbytes = Encoding.UTF8.GetBytes(length);

                    await socket.SendAsync(lengthbytes, WebSocketMessageType.Text, true, CancellationToken.None);
                    await socket.SendAsync(bytes, WebSocketMessageType.Binary, true, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    //ServerDisconnected?.Invoke(this, new ServerDisconnectedEventArgs());
                }
            }
            else
            {
                ServerDisconnected?.Invoke(this, new ServerDisconnectedEventArgs());
            }
        }
    }
}
