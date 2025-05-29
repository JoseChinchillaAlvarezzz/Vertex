using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Vertex.Models
{
    public class ticketsContext : DbContext
    {
        public ticketsContext(DbContextOptions<ticketsContext> options) : base(options)  
        {

        }
    }
}
