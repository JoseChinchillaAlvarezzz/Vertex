using Microsoft.AspNetCore.Mvc;

namespace Vertex.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
