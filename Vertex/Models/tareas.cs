using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class tareas
    {
        [Key]
        public int id { get; set; }
        public string titulo { get; set; }
        public string descripcion {  get; set; }
        public int ticket_id { get; set; }
        public int estado_tarea_id { get; set; }
    }
}
