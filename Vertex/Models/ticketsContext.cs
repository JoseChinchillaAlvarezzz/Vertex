using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Identity.Client;

namespace Vertex.Models
{
    public class ticketsContext : DbContext
    {
        public ticketsContext(DbContextOptions<ticketsContext> options) : base(options)  
        {
 
        }

        public DbSet<roles> roles { get; set; }
        public DbSet<categorias> categorias { get; set; }
        public DbSet<estados_tickets> estados_tickets { get; set; }
        public DbSet<prioridades> prioridades { get; set; }
        public DbSet<clientes> clientes { get; set; }
        public DbSet<usuarios> usuarios { get; set; }
        public DbSet<tickets> tickets { get; set; }
        public DbSet<asignaciones> asignaciones { get; set; }
        public DbSet<comentarios> comentarios { get; set; }
        public DbSet<cambios_estado> cambios_estado { get; set; }
        public DbSet<estados_tareas> estados_tareas { get; set; }
        public DbSet<tareas> tareas { get; set; }
    }
}
