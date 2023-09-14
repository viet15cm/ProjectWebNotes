using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectWebNotes.Controllers
{

    public class ChatRoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
