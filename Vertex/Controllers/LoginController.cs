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

        

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Buscar en usuarios (Administrador o Técnico)
            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(u => u.email == email && u.contrasenia == password);

            if (usuario != null)
            {
                HttpContext.Session.SetInt32("usuarioId", usuario.id);
                HttpContext.Session.SetString("nombre", usuario.nombre);
                HttpContext.Session.SetInt32("rolId", usuario.rol_id);

                if (usuario.rol_id == 1) // Administrador
                    return RedirectToAction("Index", "Admin");

                if (usuario.rol_id == 2) // Técnico
                    return RedirectToAction("Index", "Cliente");

                return RedirectToAction("Login");
            }

            // Buscar en clientes
            var cliente = await _context.clientes
                .FirstOrDefaultAsync(c => c.email == email && c.contrasenia == password);

            if (cliente != null)
            {
                HttpContext.Session.SetInt32("clienteId", cliente.id);
                HttpContext.Session.SetString("nombre", cliente.nombre);
                HttpContext.Session.SetString("apellido", cliente.apellido);
                HttpContext.Session.SetString("telefono", cliente.telefono);
                HttpContext.Session.SetString("email", cliente.email);
                HttpContext.Session.SetString("rol", "Cliente");

                return RedirectToAction("Index", "Cliente");
            }

            ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
