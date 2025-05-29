using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class cambios_estado
    {
        [Key]
        public int id { get; set; }
        public DateTime fechacambio { get; set; }
        public int ticket_id { get; set; }
    }
}
