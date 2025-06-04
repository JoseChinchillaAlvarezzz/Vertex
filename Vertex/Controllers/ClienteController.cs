using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult Crear()
        {
            var clienteId = HttpContext.Session.GetInt32("clienteId");
            var nombre = HttpContext.Session.GetString("nombre");
            var apellido = HttpContext.Session.GetString("apellido");
            var telefono = HttpContext.Session.GetString("telefono");
            var email = HttpContext.Session.GetString("email");

            ViewBag.Nombre = nombre;
            ViewBag.Apellido = apellido;
            ViewBag.Telefono = telefono;
            ViewBag.Email = email;

            ViewBag.Categorias = new SelectList(_context.categorias.ToList(), "id", "categoria");
            ViewBag.Prioridades = new SelectList(_context.prioridades.ToList(), "id", "prioridad");

            return View(new tickets());
        }


        [HttpPost]
        public IActionResult Crear(tickets ticket)
        {
            ViewBag.Categorias = new SelectList(_context.categorias.ToList(), "id", "categoria");
            ViewBag.Prioridades = new SelectList(_context.prioridades.ToList(), "id", "prioridad");

            if (!ModelState.IsValid)
                return View(ticket);

            int? clienteId = HttpContext.Session.GetInt32("clienteId");
            if (clienteId == null)
                return RedirectToAction("Login", "Login");

            ticket.fechacreacion = DateTime.Now;
            ticket.cliente_id = clienteId.Value;
            ticket.estado_ticket_id = 1;
            ticket.usuario_id = 1; 

            _context.tickets.Add(ticket);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }




    }
}
