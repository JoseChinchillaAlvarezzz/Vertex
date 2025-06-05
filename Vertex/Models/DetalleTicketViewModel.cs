namespace Vertex.Models
{
    public class DetalleTicketViewModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string categoria { get; set; }
        public string? prioridad { get; set; }
        public string aplicacion { get; set; }
        public string tecnico { get; set; }
        public string descripcion { get; set; }

        public List<tareas> tareas { get; set; }
        public List<comentarios> comentarios { get; set; }  
    }
}
