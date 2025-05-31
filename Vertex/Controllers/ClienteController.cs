using Microsoft.AspNetCore.Mvc;
using Vertex.Models;

namespace Vertex.Controllers
{
    public class ClienteController : Controller
    {

        public readonly ticketsContext _context;

        public ClienteController(ticketsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
