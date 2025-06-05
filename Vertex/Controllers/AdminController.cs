using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Vertex.Models;
using Vertex.Services;

namespace Vertex.Controllers
{
    public class AdminController : Controller
    {
        private readonly ticketsContext _context;
        private readonly IConfiguration _configuration;
        public AdminController(ticketsContext context, IConfiguration configuration) 
        {
            _context = context;
            _configuration = configuration;
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
                                     prioridad = p.prioridad,
                                     idPrioridad = t.prioridad_id 
                                 }).FirstOrDefault();

            ViewData["Tecnicos"] = new SelectList(tecnicos, "id", "nombre");
            ViewData["Prioridades"] = new SelectList(prioridades, "id", "prioridad", detalleTicket.idPrioridad);
            ViewData["detalleTicket"] = detalleTicket;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Asignar(int idTicket, int idPrioridad, int idTecnico)
        {
            // Crear asignación
            var asignacion = new asignaciones
            {
                fechaasignacion = DateTime.Today,
                usuario_id = idTecnico,
                ticket_id = idTicket
            };

            _context.asignaciones.Add(asignacion);

            // Buscar ticket y actualizar prioridad
            var ticket = _context.tickets.FirstOrDefault(t => t.id == idTicket);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.prioridad_id = idPrioridad;

            // Obtener datos del técnico para notificación
            var tecnico = _context.usuarios.FirstOrDefault(u => u.id == idTecnico);
            if (tecnico != null)
            {
                string asunto = "Nuevo Ticket Asignado - Vertex";
                string mensaje = $@"
            Hola <b>{tecnico.nombre} {tecnico.apellido}</b>,<br><br>
            Se te ha asignado un nuevo ticket:<br><br>
            <b>Título:</b> {ticket.titulo}<br>
            <b>Aplicación:</b> {ticket.aplicacion}<br>
            <b>Descripción:</b> {ticket.descripcion}<br>
            <b>Fecha de creación:</b> {ticket.fechacreacion:dd/MM/yyyy HH:mm}<br><br>
            Por favor, revisa y atiende este ticket a la brevedad.<br><br>
            <i>Equipo Vertex</i>";

                correo enviarCorreo = new correo(_configuration);
                enviarCorreo.enviar(tecnico.email, asunto, mensaje);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");
        }
        // ... (tus otros métodos) ...

        [HttpGet]
        public IActionResult CreaUsu()
        {
            // Llenar el combo de roles desde la base de datos
            var roles = _context.roles
                .Select(r => new { r.id, r.rol })
                .ToList();
            ViewBag.Roles = new SelectList(roles, "id", "rol");

            return View();
        }
        public IActionResult listausuarios()
        {
            var listaUsuarios = _context.usuarios.ToList();
            return View(listaUsuarios);
        }

        [HttpPost]
        public IActionResult CreaUsu(usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                _context.usuarios.Add(usuario);
                _context.SaveChanges();

                // Enviar correo al nuevo usuario
                string asunto = "Bienvenido a Vertex - Credenciales de acceso";
                string mensaje = $@"
            Hola <b>{usuario.nombre} {usuario.apellido}</b>,<br><br>
            Tu cuenta ha sido creada con éxito en el sistema Vertex.<br><br>
            <b>Correo:</b> {usuario.email}<br>
            <b>Contraseña:</b> {usuario.contrasenia}<br>
            <b>Rol:</b> {_context.roles.FirstOrDefault(r => r.id == usuario.rol_id)?.rol ?? "No especificado"}<br><br>
            Puedes iniciar sesión y comenzar a gestionar tus tickets.<br><br>
            <i>Equipo Vertex</i>
        ";

                correo enviarCorreo = new correo(_configuration);
                enviarCorreo.enviar(usuario.email, asunto, mensaje);

                return RedirectToAction("Index", "Admin");
            }

            // Si falla la validación, volver a llenar los roles
            var roles = _context.roles
                .Select(r => new { r.id, r.rol })
                .ToList();
            ViewBag.Roles = new SelectList(roles, "id", "rol");

            return View(usuario);
        }

        [HttpGet]
        public IActionResult EditarUsuario(int id)
        {
            var usuario = _context.usuarios.FirstOrDefault(u => u.id == id);
            if (usuario == null) return NotFound();

            // Llenar los roles para el combo
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "id", "rol", usuario.rol_id);
            return View(usuario);

        }
        [HttpPost]
        public IActionResult EditarUsuario(usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "id", "rol", usuario.rol_id);
            return View(usuario);
        }

        [HttpGet]
        public IActionResult EdiUsu(int id)
        {
            var usuario = _context.usuarios.FirstOrDefault(u => u.id == id);
            if (usuario == null) return NotFound();

            // Llenar los roles para el combo
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "id", "rol", usuario.rol_id);
            return View(usuario); // Renderiza EdiUsu.cshtml
        }

        [HttpPost]
        public IActionResult EdiUsu(usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
                return RedirectToAction("listausuarios"); // O donde prefieras
            }
            ViewBag.Roles = new SelectList(_context.roles.ToList(), "id", "rol", usuario.rol_id);
            return View(usuario);
        }






    }

}

