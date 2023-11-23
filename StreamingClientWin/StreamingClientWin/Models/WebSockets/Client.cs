using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace DjStreamer.Models.WebSockets
{
    public class Client : ConnectionBase
    {
        public Client(TaskCompletionSource<object> source, WebSocket socket) : base(source, socket)
        {

        }
    }
}
