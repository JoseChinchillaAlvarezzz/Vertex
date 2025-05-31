using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vertex.Models; // Usa tu namespace real
using System.Threading.Tasks;

namespace Vertex.Controllers
{
    public class LoginController : Controller
    {
        private readonly ticketsContext _context;

        public LoginController(ticketsContext context) 
        {
            _context = context;
        }

        public IActionResult Login() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Verificar si es un usuario interno (técnico o administrador)
            var usuario = await _context.usuarios
                .Include(u => u.rol_id)
                .FirstOrDefaultAsync(u => u.email == email && u.contrasenia == password);

            if (usuario != null)
            {
                HttpContext.Session.SetInt32("usuarioId", usuario.id);
                HttpContext.Session.SetString("nombre", usuario.nombre);
                HttpContext.Session.SetInt32("rol", usuario.rol_id);
                // Redirección según rol
                if (usuario.rol_id == 1) // Administrador
                    return RedirectToAction("Index", "Admin");
                else if (usuario.rol_id == 2) // Técnico
                    return RedirectToAction("Index", "Tecnico"); // o un controlador específico de técnicos si lo tienes

                return RedirectToAction("Login");
            }

            // Buscar en tabla de clientes
            var cliente = await _context.clientes
                .FirstOrDefaultAsync(c => c.email == email && c.contrasenia == password);

            if (cliente != null)
            {
                HttpContext.Session.SetInt32("clienteId", cliente.id);
                HttpContext.Session.SetString("nombre", cliente.nombre);
                HttpContext.Session.SetString("rol", "Cliente");

                return RedirectToAction("Index", "Cliente");
            }

            ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
