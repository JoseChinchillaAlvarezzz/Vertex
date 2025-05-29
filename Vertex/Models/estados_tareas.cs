using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class estados_tareas
    {
        [Key]
        public int id { get; set; }
        public string estado { get; set; }
    }
}
