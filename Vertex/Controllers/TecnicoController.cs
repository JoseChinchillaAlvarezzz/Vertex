using Microsoft.AspNetCore.Mvc;
using Vertex.Models;
using System.Linq;
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
            var nombre = HttpContext.Session.GetString("nombre");
            ViewBag.NombreUsuario = nombre ?? "Usuario";
            return View();
        }

        public IActionResult MisAsignaciones()
        {
            var nombre = HttpContext.Session.GetString("nombre");
            ViewBag.NombreUsuario = nombre ?? "Usuario";

            int? tecnicoId = HttpContext.Session.GetInt32("usuarioId");
            List<tickets> listadoTickets = new();

            if (tecnicoId != null)
            {
                listadoTickets = _context.asignaciones
                    .Where(a => a.usuario_id == tecnicoId)
                    .Select(a => a.ticket_id)
                    .Distinct()
                    .Join(_context.tickets,
                          id => id,
                          t => t.id,
                          (id, t) => t)
                    .Where(t => t.estado_ticket_id == 1 || t.estado_ticket_id == 2)
                    .ToList();
            }

            return View(listadoTickets);
        }

        [HttpPost]
        public IActionResult IniciarTrabajo(int id)
        {
            var ticket = _context.tickets.FirstOrDefault(t => t.id == id);
            if (ticket != null && ticket.estado_ticket_id == 1) // Solo si está pendiente
            {
                ticket.estado_ticket_id = 2; // "En progreso"
                _context.SaveChanges();
            }

            return RedirectToAction("ver_detalle", new { id = id });
        }

        public IActionResult ver_detalle(int id)
        {
            var ticket = _context.tickets
                .Include(t => t.cliente)
                .Include(t => t.categoria)
                .Include(t => t.prioridad)
                .Include(t => t.comentarios)
                .Include(t => t.tareas)
                .FirstOrDefault(t => t.id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var estado = _context.estados_tickets.FirstOrDefault(e => e.id == ticket.estado_ticket_id);
            ViewBag.Estado = estado?.estado ?? "Desconocido";

            return View(ticket); // Esto carga Views/Tecnico/ver_detalle.cshtml
        }
        [HttpPost]
        public IActionResult FinalizarTrabajo(int id)
        {
            var ticket = _context.tickets.FirstOrDefault(t => t.id == id);
            if (ticket != null && ticket.estado_ticket_id == 2) // Solo si está en progreso
            {
                ticket.estado_ticket_id = 4; // "Resuelto"
                _context.SaveChanges();
            }

            return RedirectToAction("ver_detalle", new { id = id });
        }


    }
}
