﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
                                         where (t.estado_ticket_id == 1 || t.estado_ticket_id == 2 || t.estado_ticket_id == 3)
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

                
                correo enviarCorreo = new correo(_configuration);
                string asunto = "Actualización de Datos - Sistema Vertex";
                string mensaje = $@"
            Hola <b>{usuario.nombre} {usuario.apellido}</b>,<br><br>
            Tus datos han sido actualizados correctamente en el sistema Vertex.<br><br>
            <b>Correo:</b> {usuario.email}<br>
            <b>Teléfono:</b> {usuario.telefono}<br>
            <b>Rol:</b> {(_context.roles.FirstOrDefault(r => r.id == usuario.rol_id)?.rol ?? "Desconocido")}<br><br>
            Si no solicitaste esta modificación, por favor contáctanos.<br><br>
            <i>Equipo Vertex</i>
        ";
                enviarCorreo.enviar(usuario.email, asunto, mensaje);

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

        [HttpGet]
        public IActionResult Detalle(int idTicket)
        {
            var detalleTicket = (from t in _context.tickets
                                 join c in _context.clientes on t.cliente_id equals c.id
                                 join p in _context.prioridades on t.prioridad_id equals p.id
                                 join ca in _context.categorias on t.categoria_id equals ca.id
                                 join a in _context.asignaciones on t.id equals a.ticket_id
                                 join u in _context.usuarios on a.usuario_id equals u.id
                                 where t.id == idTicket
                                 select new DetalleTicketViewModel
                                 {
                                     id = t.id,
                                     nombre = c.nombre,
                                     apellido = c.apellido,
                                     telefono = c.telefono,
                                     correo = c.email,
                                     categoria = ca.categoria,
                                     prioridad = p.prioridad,
                                     aplicacion = t.aplicacion,
                                     tecnico = u.nombre + " " + u.apellido,
                                     descripcion = t.descripcion
                                 }).FirstOrDefault();

            var comentarios = (from c in _context.comentarios
                               where c.ticket_id == idTicket
                               select new ComentarioViewModel
                               {
                                   id = c.id,
                                   titulo = c.titulo
                               }).ToList();

            var tareas = (from t in _context.tareas
                          where t.ticket_id == idTicket
                          select new TareaViewModel
                          {
                              id = t.id,
                              titulo = t.titulo
                          }).ToList();

            var viewModel = new DetalleTicketCompletoViewModel
            {
                Detalle = detalleTicket,
                Comentarios = comentarios,
                Tareas = tareas
            };

            ViewData["detalleTicket"] = detalleTicket;
            ViewData["Comentarios"] = comentarios;
            ViewData["Tareas"] = tareas;

            return View(viewModel);
        }
        public IActionResult DetalleTic(int id)
        {

            {
                var ticket = _context.tickets
                    .Include(t => t.cliente)
                    .Include(t => t.categoria)
                    .Include(t => t.prioridad)
                    .FirstOrDefault(t => t.id == id);

                if (ticket == null)
                    return NotFound();

                // Tareas y comentarios asociados
                ticket.tareas = _context.tareas.Where(t => t.ticket_id == id).ToList();
                ticket.comentarios = _context.comentarios.Where(c => c.ticket_id == id).ToList();

                // Obtener técnicos (para el select de técnico encargado)
                var tecnicos = _context.usuarios
                    .Where(u => u.rol_id == 2)
                    .Select(u => new { id = u.id, Nombre = u.nombre + " " + u.apellido })
                    .ToList();

                // Técnico encargado (última asignación para este ticket)
                int? tecnicoEncargadoId = _context.asignaciones
                    .Where(a => a.ticket_id == id)
                    .OrderByDescending(a => a.fechaasignacion)
                    .Select(a => (int?)a.usuario_id)
                    .FirstOrDefault();

                ViewBag.Tecnicos = tecnicos;
                ViewBag.TecnicoEncargadoId = tecnicoEncargadoId;

                return View(ticket);


            }
        }
        public IActionResult InfoTick()
        {

            // Supongamos que estado_ticket_id = 4 es "Completado"
            var ticketsCompletados = _context.tickets
                .Where(t => t.estado_ticket_id == 4)
                .OrderByDescending(t => t.fechacreacion)
                .ToList();

            return View(ticketsCompletados);
        }

        [HttpGet]
        public IActionResult AgregarComentarioAdmin(int ticketId)
        {
            var ticket = (from t in _context.tickets
                          where t.id == ticketId
                          select t).FirstOrDefault();

            ViewData["ticket"] = ticket.id;
            return View();
        }

        [HttpPost]
        public IActionResult AgregarComentarioAdmin(int ticketId, string titulo, string comentario)
        {
            var adminId = HttpContext.Session.GetInt32("usuarioId");

            var nuevoComentario = new comentarios
            {
                titulo = titulo,
                comentario = comentario,
                ticket_id = ticketId,
                usuario_id = adminId.Value,
            };

            _context.comentarios.Add(nuevoComentario);
            _context.SaveChanges();

            return RedirectToAction("Index", "Admin", new { id = ticketId });
        }

        [HttpGet]
        public IActionResult AgregarTarea(int ticketId) 
        {
            var ticket = (from t in _context.tickets
                          where t.id == ticketId
                          select t).FirstOrDefault();

            ViewData["ticket"] = ticket.id;
            return View();
        }

        [HttpPost]
        public IActionResult AgregarTarea(int ticketId, string titulo, string descripcion) 
        {
            var adminId = HttpContext.Session.GetInt32("usuarioId");

            var nuevaTarea = new tareas
            {
                titulo = titulo,
                descripcion = descripcion,
                ticket_id = ticketId,
                estado_tarea_id = 1
            };

            _context.tareas.Add(nuevaTarea);
            _context.SaveChanges();

            return RedirectToAction("Index", "Admin", new { id = ticketId });
        }

        public IActionResult Dashboard()
        {
            // 1) DISTRIBUCIÓN POR PRIORIDAD (conteo total de tickets por prioridad)
            var priorityCounts = _context.tickets
                .Include(t => t.prioridad)
                .GroupBy(t => t.prioridad.prioridad)
                .Select(g => new
                {
                    PriorityName = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(x => x.PriorityName, x => x.Count);

            // 2) TICKETS ÚLTIMOS 30 DÍAS POR APLICACIÓN
            var fromDate = DateTime.Today.AddDays(-30);
            var appCounts = _context.tickets
                .Where(t => t.fechacreacion >= fromDate &&
                            !string.IsNullOrEmpty(t.aplicacion))
                .GroupBy(t => t.aplicacion)
                .Select(g => new
                {
                    AppName = g.Key!,
                    Count = g.Count()
                })
                .ToDictionary(x => x.AppName, x => x.Count);

            // 3) TOP TÉCNICOS CON MÁS TICKETS RESUELTOS
            //    Hacemos un join manual: asignaciones → tickets → usuarios
            var techResolved = (
                from a in _context.asignaciones
                join t in _context.tickets on a.ticket_id equals t.id
                where t.estado_ticket_id == 4              // solo “Resuelto”
                group a by a.usuario_id into grp
                select new
                {
                    TechnicianId = grp.Key,
                    Count = grp.Count()
                }
            )
            .Join(
                _context.usuarios,
                grp => grp.TechnicianId,
                u => u.id,
                (grp, u) => new
                {
                    FullName = u.nombre + " " + u.apellido,
                    Count = grp.Count
                }
            )
            .ToDictionary(x => x.FullName, x => x.Count);

            // Empaquetamos en el ViewModel
            var vm = new DashboardViewModel
            {
                PriorityCounts = priorityCounts,
                ApplicationCountsLast30Days = appCounts,
                TechnicianResolvedCounts = techResolved
            };

            return View(vm);
        }
    }
}
