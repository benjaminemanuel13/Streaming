using DjStreamer.Business.Services;
using DjStreamer.Business.Services.Interfaces;
using System.Net.WebSockets;

namespace DjStreamer.Models.WebSockets
{
    public class Server : ConnectionBase
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public List<Client> Clients { get; set; } = new List<Client>();

        private int count = 0;
        private List<byte> buffer = new List<byte>();

        private object obj = new object();

        public bool Removing { get; set; } = false;

        public Server(IWebHostEnvironment host, string id, string userId, TaskCompletionSource<object> source, WebSocket socket) : base(host, source, socket, true)
        {
            Id = id;
            UserId = userId;
        }

        public void Close() 
        {
            
        }

        protected async override void RecieveData(byte[] bytes)
        {
            if (count++ < 125)
            {
                buffer.AddRange(bytes);
                return;
            }

            count = 0;
            byte[] data = buffer.ToArray();

            while (Removing)
            { }

            foreach (var client in Clients)
            {
                await client.SendData(data);
            }
            
            //stream.Write(data);
            //stream.Flush();

            buffer.Clear();
        }
    }
}
