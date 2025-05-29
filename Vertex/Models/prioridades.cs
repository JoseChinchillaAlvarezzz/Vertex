using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class prioridades
    {
        [Key]
        public int id { get; set; }
        public string prioridad { get; set; }
    }
}
