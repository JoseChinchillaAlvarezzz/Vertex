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
            
            int? clienteId = HttpContext.Session.GetInt32("clienteId");
            if (clienteId == null)
                return RedirectToAction("Login", "Login");

            
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
            ticket.estado_ticket_id = 1; 
            ticket.usuario_id = 1; 

            _context.tickets.Add(ticket);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Detalle(int id)
        {
            var ticket = _context.tickets.FirstOrDefault(t => t.id == id);
            if (ticket == null) return NotFound();

            // Para mostrar texto legible en vez de IDs
            ViewBag.Categoria = _context.categorias.Find(ticket.categoria_id)?.categoria ?? "Desconocida";
            ViewBag.Prioridad = _context.prioridades.Find(ticket.prioridad_id)?.prioridad ?? "Desconocida";
            ViewBag.Estado = _context.estados_tickets.Find(ticket.estado_ticket_id)?.estado ?? "Desconocido";

            return View(ticket);
        }





    }
}
