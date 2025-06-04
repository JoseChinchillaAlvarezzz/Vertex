using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vertex.Models
{
    public class comentarios
    {
        [Key]
        public int id { get; set; }
        public string titulo { get; set; }
        public string comentario { get; set; }
        public int ticket_id { get; set; }
        public int usuario_id { get; set; }


        [ForeignKey("ticket_id")]
        public virtual tickets ticket { get; set; }

    }

}
