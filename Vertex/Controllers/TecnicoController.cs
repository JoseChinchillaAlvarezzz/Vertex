using Microsoft.AspNetCore.Mvc;

namespace Vertex.Controllers
{
    public class TecnicoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
