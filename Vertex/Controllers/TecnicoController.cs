using Microsoft.AspNetCore.Mvc;
using Vertex.Models;
using Microsoft.EntityFrameworkCore;

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
            var nombreUsuario = HttpContext.Session.GetString("nombre");
            ViewBag.NombreUsuario = nombreUsuario;

            int? tecnicoId = HttpContext.Session.GetInt32("usuarioId");

            List<tickets> listaTickets = new();

            if (tecnicoId != null)
            {
                listaTickets = _context.asignaciones
                    .Where(a => a.usuario_id == tecnicoId)
                    .Select(a => a.ticket_id)
                    .Distinct()
                    .Join(_context.tickets,
                        id => id,
                        t => t.id,
                        (id,t) => t)
                    .Where(t => t.estado_ticket_id == 1)
                    .ToList();
            }

            return View(listaTickets);
        }
        public IActionResult VerDetalle(int id)
        {
            // 1. Buscar el ticket
            var ticket = _context.tickets.FirstOrDefault(t => t.id == id);
            if (ticket == null)
                return NotFound();

            // 2. Cargar manualmente los datos relacionados
            var categoria = _context.categorias.FirstOrDefault(c => c.id == ticket.categoria_id);
            var prioridad = _context.prioridades.FirstOrDefault(p => p.id == ticket.prioridad_id);
            var estado = _context.estados_tickets.FirstOrDefault(e => e.id == ticket.estado_ticket_id);
            var cliente = _context.clientes.FirstOrDefault(c => c.id == ticket.cliente_id);

            // 3. Enviar todo por ViewBag como hiciste con nombreUsuario
            ViewBag.Categoria = categoria?.categoria ?? "Sin categoría";
            ViewBag.Prioridad = prioridad?.prioridad ?? "Sin prioridad";
            ViewBag.Estado = estado?.estado ?? "Sin estado";
            ViewBag.Cliente = cliente != null ? $"{cliente.nombre} {cliente.apellido}" : "Sin cliente";

            return View(ticket);
        }

    }
}