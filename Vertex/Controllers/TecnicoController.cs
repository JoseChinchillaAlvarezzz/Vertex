using Microsoft.AspNetCore.Mvc;

namespace Vertex.Controllers
{
    public class TecnicoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Tickets()
        {
            // Aquí puedes pasar un modelo si luego lo necesitas
            return View();
        }
    }
}