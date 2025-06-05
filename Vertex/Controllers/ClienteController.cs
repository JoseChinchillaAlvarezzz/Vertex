using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vertex.Models;
using Vertex.Services;

namespace Vertex.Controllers
{
    public class ClienteController : Controller
    {

        public readonly ticketsContext _context;
        private IConfiguration _configuration;

        public ClienteController(ticketsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            Random rnd = new Random();
            correo enviarcorreo = new correo(_configuration);

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
            ticket.usuario_id = rnd.Next(1, 3); 
                                               

            _context.tickets.Add(ticket);
            _context.SaveChanges();

            // OBTENER datos de cliente desde la base de datos
            var cliente = _context.clientes.FirstOrDefault(c => c.id == clienteId);
            if (cliente != null)
            {
                string correoDestino = cliente.email;
                string asunto = "Confirmación de Ticket - Vertex";
                string mensaje = $@"
            Hola <b>{cliente.nombre} {cliente.apellido}</b>,<br><br>
            Hemos recibido tu ticket:<br>
            <b>Título:</b> {ticket.titulo}<br>
            <b>Aplicación:</b> {ticket.aplicacion}<br>
            <b>Descripción:</b> {ticket.descripcion}<br>
            <b>Fecha:</b> {ticket.fechacreacion:dd/MM/yyyy HH:mm}<br><br>
            Te contactaremos pronto. Gracias por tu confianza.<br><br>
            <i>Equipo Vertex</i>
        ";

                enviarcorreo.enviar(correoDestino, asunto, mensaje);
            }

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
