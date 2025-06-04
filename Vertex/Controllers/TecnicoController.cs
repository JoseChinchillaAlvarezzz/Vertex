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
            return View();
        }

        public IActionResult Tickets()
        {
            // Trae todos los tickets, puedes filtrar por técnico si lo deseas
            var listadoTickets = _context.tickets.ToList();
            return View(listadoTickets);
        }
    }
}
