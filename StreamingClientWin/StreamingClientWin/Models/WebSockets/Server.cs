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

        public Server(string id, string userId, TaskCompletionSource<object> source, WebSocket socket) : base(source, socket, true)
        {
            Id = id;
            UserId = userId;
        }

        protected override void RecieveData(byte[] bytes)
        {
            //if (count++ < 70)
            //{
            //    buffer.AddRange(bytes);
            //    return;
            //}

            //count = 0;
            //byte[] data = buffer.ToArray();

            foreach (var client in Clients)
            {
                //client.SendData(data);
                client.SendData(bytes);
            }

            buffer.Clear();
        }
    }
}
