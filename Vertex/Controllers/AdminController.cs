using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vertex.Models;

namespace Vertex.Controllers
{
    public class AdminController : Controller
    {
        private readonly ticketsContext _context;
        public AdminController(ticketsContext context) 
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var adminId = HttpContext.Session.GetInt32("usuarioId");
            var nombre = HttpContext.Session.GetString("nombre");

            ViewBag.Bienvenida = "Bienvenido " + nombre;

            var listadoTicketsPendientes = (from t in _context.tickets
                                            join p in _context.prioridades on t.prioridad_id equals p.id
                                            where t.usuario_id == adminId
                                            && !_context.asignaciones.Any(a => a.ticket_id == t.id)
                                            select new
                                            {
                                                id = t.id,
                                                titulo = t.titulo,
                                                prioridad = p.prioridad
                                            }).ToList();

            var listadoTicketsActivos = (from t in _context.tickets
                                         join p in _context.prioridades on t.prioridad_id equals p.id
                                         where (t.estado_ticket_id == 1 || t.estado_ticket_id == 2 || t.estado_ticket_id ==3)
                                         && t.usuario_id == adminId
                                         && _context.asignaciones.Any(a => a.ticket_id == t.id)
                                         select new 
                                         {
                                             id = t.id,
                                             titulo = t.titulo,
                                             prioridad = p.prioridad
                                         }).ToList();

            ViewData["listadoTicketsPendientes"] = listadoTicketsPendientes;
            ViewData["listadoTicketsActivos"] = listadoTicketsActivos;

            return View();
        }

        [HttpGet]
        public IActionResult Asignar(int idTicket) 
        {
            var tecnicos = (from u in _context.usuarios
                            where u.rol_id == 2
                            select new
                            {
                                id = u.id,
                                nombre = u.nombre + " " + u.apellido
                            }).ToList();

            var prioridades = (from p in _context.prioridades
                               select new
                               {
                                   id = p.id,
                                   prioridad = p.prioridad
                               }).ToList();

            var detalleTicket = (from t in _context.tickets
                                 join cl in _context.clientes on t.cliente_id equals cl.id
                                 join ca in _context.categorias on t.categoria_id equals ca.id
                                 join p in _context.prioridades on t.prioridad_id equals p.id
                                 where t.id == idTicket
                                 select new AsignarTicketViewModel
                                 {
                                     id = t.id,
                                     nombre = cl.nombre,
                                     apellido = cl.apellido,
                                     telefono = cl.telefono,
                                     email = cl.email,
                                     descripcion = t.descripcion,
                                     aplicacion = t.aplicacion,
                                     categoria = ca.categoria,
                                     prioridad = p.prioridad
                                 }).FirstOrDefault();

            ViewData["Tecnicos"] = new SelectList(tecnicos, "id", "nombre");
            ViewData["Prioridades"] = new SelectList(prioridades, "id", "prioridad");
            ViewData["detalleTicket"] = detalleTicket;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Asignar(int idTicket, int idPrioridad, int idTecnico) 
        {
            //Creacion de asignacion
            var asignacion = new asignaciones
            {
                fechaasignacion = DateTime.Today,
                usuario_id = idTecnico,
                ticket_id = idTicket
            };

            _context.asignaciones.Add(asignacion);

            //Actualizacion de prioridad
            var ticket = (from t in _context.tickets
                          where t.id == idTicket select t).FirstOrDefault();

            Console.WriteLine(ticket.id);

            if (ticket == null)
            {
                return NotFound(); 
            }

            ticket.prioridad_id = idPrioridad;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Admin", new { id = idTicket });
        }

    }
}
