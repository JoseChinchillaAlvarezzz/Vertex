using Microsoft.AspNetCore.Mvc;
using Vertex.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using Vertex.Services;

namespace Vertex.Controllers
{
    public class TecnicoController : Controller
    {
        private readonly ticketsContext _context;
        private IConfiguration _configuration;
        public TecnicoController(ticketsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

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
            if (ticket != null && ticket.estado_ticket_id == 1)
            {
                ticket.estado_ticket_id = 2;
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

            return View(ticket);
        }

        [HttpPost]
        public IActionResult FinalizarTrabajo(int id)
        {
            correo enviarcorreo = new correo(_configuration);
            var ticket = _context.tickets.FirstOrDefault(t => t.id == id);
            if (ticket != null && ticket.estado_ticket_id == 2)
            {
                ticket.estado_ticket_id = 4;
                _context.SaveChanges();

                // Obtener datos del cliente o usuario
                string destinatario = "";
                string nombreCompleto = "";
                if (ticket.cliente_id != null)
                {
                    var cliente = _context.clientes.FirstOrDefault(c => c.id == ticket.cliente_id);
                    if (cliente != null)
                    {
                        destinatario = cliente.email;
                        nombreCompleto = cliente.nombre + " " + cliente.apellido;
                    }
                }
                else if (ticket.usuario_id != null)
                {
                    var usuario = _context.usuarios.FirstOrDefault(u => u.id == ticket.usuario_id);
                    if (usuario != null)
                    {
                        destinatario = usuario.email;
                        nombreCompleto = usuario.nombre + " " + usuario.apellido;
                    }
                }

                // Si se encontró un destinatario, enviar correo
                if (!string.IsNullOrEmpty(destinatario))
                {
                    string asunto = "Finalización de Ticket - Vertex";
                    string mensaje = $@"
                Hola <b>{nombreCompleto}</b>,<br><br>
                Te informamos que tu ticket ha sido finalizado exitosamente:<br><br>
                <b>Número de Ticket:</b> {ticket.id}<br>
                <b>Título:</b> {ticket.titulo}<br>
                <b>Aplicación:</b> {ticket.aplicacion}<br>
                <b>Descripción:</b> {ticket.descripcion}<br>
                <b>Fecha de creación:</b> {ticket.fechacreacion:dd/MM/yyyy HH:mm}<br><br>
                Estado actual: <b>Finalizado</b><br><br>
                Gracias por utilizar nuestro sistema.<br><br>
                <i>Equipo Vertex</i>";

                    correo enviarCorreo = new correo(_configuration);
                    enviarCorreo.enviar(destinatario, asunto, mensaje);
                }
            }

            return RedirectToAction("ver_detalle", new { id = id });
        }


        [HttpGet]
        public IActionResult AgregarComentario(int ticketId)
        {
            var ticket = _context.tickets.FirstOrDefault(t => t.id == ticketId);
            if (ticket == null) return NotFound();

            ViewBag.TicketId = ticketId;
            return View();
        }

        [HttpPost]
        public IActionResult AgregarComentario(int ticketId, string titulo, string comentario)
        {
            int? tecnicoId = HttpContext.Session.GetInt32("usuarioId");
            if (tecnicoId == null)
                return RedirectToAction("Login", "Login");

            var nuevoComentario = new comentarios
            {
                titulo = titulo,
                comentario = comentario,
                ticket_id = ticketId,
                usuario_id = tecnicoId.Value
            };

            _context.comentarios.Add(nuevoComentario);
            _context.SaveChanges();

            return RedirectToAction("ver_detalle", new { id = ticketId });
        }

        [HttpPost]
        public IActionResult Historial(string aplicacion, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var tecnicoId = HttpContext.Session.GetInt32("usuarioId");
            if (tecnicoId == null)
                return RedirectToAction("Login", "Login");

            var tickets = _context.asignaciones
                .Where(a => a.usuario_id == tecnicoId)
                .Select(a => a.ticket_id)
                .Distinct()
                .Join(_context.tickets,
                      id => id,
                      t => t.id,
                      (id, t) => t)
                .Where(t => t.estado_ticket_id == 4) 
                .Include(t => t.prioridad)
                .AsQueryable();

            if (!string.IsNullOrEmpty(aplicacion))
                tickets = tickets.Where(t => t.aplicacion.Contains(aplicacion));

            if (fechaInicio.HasValue)
                tickets = tickets.Where(t => t.fechacreacion >= fechaInicio.Value);

            if (fechaFin.HasValue)
                tickets = tickets.Where(t => t.fechacreacion <= fechaFin.Value);

            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre");
            ViewBag.Aplicacion = aplicacion;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            return View("Historial", tickets.ToList());
        }


        [HttpGet]
        public IActionResult Historial()
        {
            var tecnicoId = HttpContext.Session.GetInt32("usuarioId");
            if (tecnicoId == null)
                return RedirectToAction("Login", "Login");

            var tickets = _context.asignaciones
                .Where(a => a.usuario_id == tecnicoId)
                .Select(a => a.ticket_id)
                .Distinct()
                .Join(_context.tickets,
                      id => id,
                      t => t.id,
                      (id, t) => t)
                .Where(t => t.estado_ticket_id == 4) // Solo los resueltos
                .Include(t => t.prioridad)
                .ToList();

            ViewBag.NombreUsuario = HttpContext.Session.GetString("nombre");
            return View(tickets);
        }




        // ----------- MÉTODOS NUEVOS PARA CREAR TICKET -----------


        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Categorias = _context.categorias
                .Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.categoria })
                .ToList();

            ViewBag.Prioridades = _context.prioridades
                .Select(p => new SelectListItem { Value = p.id.ToString(), Text = p.prioridad })
                .ToList();

            return View();
        }


        [HttpPost]
        public IActionResult Crear(tickets ticket)
        {
            int? tecnicoId = HttpContext.Session.GetInt32("usuarioId");
            if (tecnicoId == null)
                return RedirectToAction("Index", "Tecnico");

            if (ModelState.IsValid)
            {
                ticket.usuario_id = new Random().Next(1, 3);
                ticket.cliente_id = 1;
                ticket.fechacreacion = DateTime.Now;
                ticket.estado_ticket_id = 1;

                _context.tickets.Add(ticket);
                _context.SaveChanges();

                TempData["Success"] = "Ticket generado correctamente.";
                return RedirectToAction("Index"); // ← esta es la redirección a la vista anterior
            }

            // Si ModelState falla, recargar dropdowns
            ViewBag.Categorias = _context.categorias
                .Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.categoria })
                .ToList();

            ViewBag.Prioridades = _context.prioridades
                .Select(p => new SelectListItem { Value = p.id.ToString(), Text = p.prioridad })
                .ToList();

            return View(ticket);
        }




        private void CargarCombos()
        {
            ViewBag.Categorias = _context.categorias
                .Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.categoria })
                .ToList();

            ViewBag.Prioridades = _context.prioridades
                .Select(p => new SelectListItem { Value = p.id.ToString(), Text = p.prioridad })
                .ToList();
        }






    }
}
