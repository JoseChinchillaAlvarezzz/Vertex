using System.ComponentModel.DataAnnotations;

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
    }
}
}
