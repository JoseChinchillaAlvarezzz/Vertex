using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vertex.Models
{
    [Table("tickets")]
    public class tickets
    {
        [Key]
        public int id { get; set; }

        public string titulo { get; set; }
        public string descripcion { get; set; }

        [Column("aplicacion")]
        public string? aplicacion { get; set; }

        [Column("fechacreacion")]
        public DateTime fechacreacion { get; set; }

        [Column("usuario_id")]
        public int usuario_id { get; set; }

        [Column("cliente_id")]
        public int cliente_id { get; set; }

        [Column("categoria_id")]
        public int? categoria_id { get; set; }

        [Column("estado_ticket_id")]
        public int estado_ticket_id { get; set; }

        [Column("prioridad_id")]
        public int? prioridad_id { get; set; }

        // Relaciones de navegación
        [ForeignKey("cliente_id")]
        public virtual clientes? cliente { get; set; }

        [ForeignKey("categoria_id")]
        public virtual categorias? categoria { get; set; }

        [ForeignKey("prioridad_id")]
        public virtual prioridades? prioridad { get; set; }

        // 🔥 MARCAR COMO [NotMapped] para evitar errores si no están mapeadas en DB
        [NotMapped]
        public virtual ICollection<comentarios>? comentarios { get; set; }

        [NotMapped]
        public virtual ICollection<tareas>? tareas { get; set; }
    }
}
