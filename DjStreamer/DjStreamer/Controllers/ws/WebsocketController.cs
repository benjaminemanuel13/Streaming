using DjStreamer.Business.Services.Interfaces;
using DjStreamer.Models.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DjStreamer.Controllers.ws
{
    [Route("ws/[controller]")]
    [ApiController]
    public class WebsocketController : ControllerBase
    {
        private readonly IDjUserService _dj;
        private readonly IWebHostEnvironment _host;

        public WebsocketController(IDjUserService dj, IWebHostEnvironment _host) 
        {
            _dj = dj;
            _host = _host;
        }

        [HttpGet("ws/Websocket/Client")]
        public async Task Client(string serverId = "Server1")
        {
            //if (!Control.Servers.ContainsKey(serverId))
            //{
            //    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            //}

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                Client client = new Client(_host, socketFinishedTcs, webSocket);

                Control.Servers[serverId].Clients.Add(client);

                await socketFinishedTcs.Task;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [HttpGet("/ws/Websocket/Server")]
        public async Task Server(string key, string userId)
        {
            //if (Control.Servers.Where(x => x.Value.UserId == userId).Any() || !_dj.KeyMatches(key, userId))
            //{
            //    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            //}

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                var newId = Guid.NewGuid().ToString();

                //Server server = new Server(newId, userId, socketFinishedTcs, webSocket);
                Server server = new Server(_host, "Server1", userId, socketFinishedTcs, webSocket);

                Control.Servers.Add("Server1", server);

                await socketFinishedTcs.Task;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
