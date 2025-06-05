namespace Vertex.Models
{
    public class DetalleTicketCompletoViewModel
    {
        public DetalleTicketViewModel Detalle { get; set; }
        public List<ComentarioViewModel> Comentarios { get; set; }
        public List<TareaViewModel> Tareas { get; set; }
    }
}
