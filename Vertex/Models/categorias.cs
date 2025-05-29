using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class categorias
    {
        [Key]
        public int id { get; set; }
        public string categoria { get; set; }
    }
}
