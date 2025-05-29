using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class roles
    {
        [Key]
        public int id { get; set; }
        public string rol { get; set; }
    }
}
