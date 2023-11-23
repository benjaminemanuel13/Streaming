using DjStreamer.Models.Session;
using Microsoft.AspNetCore.Mvc;

namespace DjStreamer.Controllers
{
    public class StreamingController : Controller
    {
        public IActionResult Index()
        {
            ClientSession session = new ClientSession();

#if DEBUG
            session.WebsocketUrl = "wss://localhost:7130/ws/Client";
#else
            session.WebsocketUrl = "wss://smile-stream.club/ws/Client";
#endif

            return View(session);
        }
    }
}
