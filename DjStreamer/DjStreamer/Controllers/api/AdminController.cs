using DjStreamer.Business.Services;
using DjStreamer.Business.Services.Interfaces;
using DjStreamer.Models.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DjStreamer.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILoggingService _log;

        public AdminController(IWebHostEnvironment host)
        {
            _log = new LoggingService(host.WebRootPath);
        }

        [HttpGet("GetServers")]
        public List<Server> GetServers()
        {
            return Control.Servers.Values.ToList();
        }

        [HttpGet("ClearServers")]
        public void ClearServers()
        {
            foreach (var server in Control.Servers)
            {
                foreach (var client in server.Value.Clients)
                {
                    client.EndConnection();
                }
            }

            for (int i = Control.Servers.Count - 1; i > 0; i--)
            {
                Control.Servers.ElementAt(i).Value.EndConnection();
            }

            Control.Servers.Clear();
        }

        [HttpGet("ClearLog")]
        public void ClearLog()
        {
            _log.ClearLog();
        }

        [HttpGet("GetLog")]
        public string GetLog()
        { 
            return _log.GetLog();
        }
    }
}
