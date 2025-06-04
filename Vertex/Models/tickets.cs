using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int cliente_id { get; set; }
        public int? categoria_id { get; set; }
        public int estado_ticket_id { get; set; }
        public int? prioridad_id { get; set; }

        // ✅ Relaciones de navegación
        [ForeignKey("cliente_id")]
        public virtual clientes cliente { get; set; }

        [ForeignKey("categoria_id")]
        public virtual categorias categoria { get; set; }

        [ForeignKey("prioridad_id")]
        public virtual prioridades prioridad { get; set; }

        public virtual ICollection<comentarios> comentarios { get; set; }
        public virtual ICollection<tareas> tareas { get; set; }

    }
}
