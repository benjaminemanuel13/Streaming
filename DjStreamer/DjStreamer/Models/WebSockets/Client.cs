using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace DjStreamer.Models.WebSockets
{
    public class Client : ConnectionBase
    {
        public Client(IWebHostEnvironment host, TaskCompletionSource<object> source, WebSocket socket) : base(host, source, socket)
        {

        }
    }
}
