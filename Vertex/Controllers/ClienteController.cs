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
                    
                    listaTickets = _context.tickets
                        .Where(t => t.usuario_id == usuarioId)
                        .ToList();
                }
                else if (clienteId != null)
                {
                    
                    listaTickets = _context.tickets
                        .Where(t => t.cliente_id == clienteId)
                        .ToList();
                }

                return View(listaTickets);
            
        }

        public IActionResult Crear()
        {
            // Validar sesión activa
            int? clienteId = HttpContext.Session.GetInt32("clienteId");
            if (clienteId == null)
                return RedirectToAction("Login", "Login");

            // Obtener datos del cliente actual
            var cliente = _context.clientes.Find(clienteId.Value);

            ViewBag.Nombre = cliente.nombre;
            ViewBag.Apellido = cliente.apellido;
            ViewBag.Telefono = cliente.telefono;
            ViewBag.Correo = cliente.email;

            ViewBag.Categorias = new SelectList(_context.categorias.ToList(), "id", "categoria");
            ViewBag.Prioridades = new SelectList(_context.prioridades.ToList(), "id", "prioridad");

            return View();
        }



        [HttpPost]
        public IActionResult Crear(tickets ticket)
        {
            int? clienteId = HttpContext.Session.GetInt32("clienteId");
            if (clienteId == null)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(_context.categorias.ToList(), "id", "categoria");
                ViewBag.Prioridades = new SelectList(_context.prioridades.ToList(), "id", "prioridad");
                return View(ticket);
            }

            ticket.fechacreacion = DateTime.Now;
            ticket.cliente_id = clienteId.Value;
            ticket.estado_ticket_id = 1; // "Pendiente"
            ticket.usuario_id = 1; // Técnico provisional

            _context.tickets.Add(ticket);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }






    }
}
