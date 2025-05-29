using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class asignaciones
    {
        [Key]
        public int id { get; set; }
        public DateTime fechaasignacion { get; set; }
        public int ticket_id { get; set; }
        public int usuario_id { get; set; }
    }
}
