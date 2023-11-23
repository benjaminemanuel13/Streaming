using DjStreamer.Business.Services;
using DjStreamer.Business.Services.Interfaces;
using DjStreamer.Models.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DjStreamer.Controllers.api
{
    [Route("ws/[controller]")]
    [ApiController]
    public class StreamingController : ControllerBase
    {
        private readonly IWebHostEnvironment _host;
        protected readonly ILoggingService _log;


        public StreamingController(IWebHostEnvironment host)
        {
            _host = host;
            _log = new LoggingService(_host.WebRootPath);
        }

        [HttpGet("/ws/Server")]
        public async Task Server()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                Server server = new Server(_host, "ID", "BEN", socketFinishedTcs, webSocket);
                Control.Servers.Add("Server1", server);

                await socketFinishedTcs.Task;

                _log.Log("Exited Server Via Task Finished: " + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString());

                foreach (var client in Control.Servers["Server1"].Clients)
                {
                    client.EndConnection();
                }

                for (int i = Control.Servers["Server1"].Clients.Count() - 1; i > 0; i--)
                { 
                    Control.Servers["Server1"].Clients[i].EndConnection();
                }

                Control.Servers.Remove("Server1");
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [HttpGet("/ws/Client")]
        public async Task Client()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                Server server = Control.Servers["Server1"];
                Client client = new Client(_host, socketFinishedTcs, webSocket);

                server.Clients.Add(client);

                await socketFinishedTcs.Task;

                _log.Log("Exited Client Via Task Finished" + DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString());

                Control.Servers["Server1"].Removing = true;
                Control.Servers["Server1"].Clients.Remove(client);
                Control.Servers["Server1"].Removing = false;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
