using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class tickets
    {
        [Key]
        public int id { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string? aplicacion { get; set; }
        public DateTime fechacreacion { get; set; }
        public int usuario_id { get; set; }
        public int? categoria_id { get; set; }
        public int estado_ticket_id { get; set; }
        public int? prioridad_id { get; set; }
        public int cliente_id { get; set; }

    }
}
