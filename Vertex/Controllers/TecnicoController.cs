using Microsoft.AspNetCore.Mvc;
using Vertex.Models;
using System.Linq;

namespace Vertex.Controllers
{
    public class TecnicoController : Controller
    {
        private readonly ticketsContext _context;

        public TecnicoController(ticketsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var nombre = HttpContext.Session.GetString("nombre");
            ViewBag.NombreUsuario = nombre ?? "Usuario";
            return View();
        }

        public IActionResult MisAsignaciones()
        {
            var nombre = HttpContext.Session.GetString("nombre");
            ViewBag.NombreUsuario = nombre ?? "Usuario";

            var listadoTickets = _context.tickets.ToList();
            return View(listadoTickets);
        }
        public IActionResult ver_detalle () 
        
        
        
        
        { return View(); }
    }
}
