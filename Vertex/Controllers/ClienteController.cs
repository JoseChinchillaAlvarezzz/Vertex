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
            
                var nombreUsuario = HttpContext.Session.GetString("nombre");
                ViewBag.NombreUsuario = nombreUsuario;

                int? usuarioId = HttpContext.Session.GetInt32("usuarioId");
                int? clienteId = HttpContext.Session.GetInt32("clienteId");

                List<tickets> listaTickets = new();

                if (usuarioId != null)
                {
                    // Tickets del técnico
                    listaTickets = _context.tickets
                        .Where(t => t.usuario_id == usuarioId)
                        .ToList();
                }
                else if (clienteId != null)
                {
                    // Tickets del cliente
                    listaTickets = _context.tickets
                        .Where(t => t.cliente_id == clienteId)
                        .ToList();
                }

                return View(listaTickets);
            
        }
    }
}
