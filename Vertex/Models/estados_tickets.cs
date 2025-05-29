using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class estados_tickets
    {
        [Key]
        public int id { get; set; }
        public string estado { get; set; }
    }
}
