using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class rol
    {
        [Key]
        public int id { get; set; }
        public string rol { get; set; }
    }
}
