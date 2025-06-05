using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vertex.Models
{
    public class AsignarTicketViewModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string descripcion { get; set; }
        public string aplicacion { get; set; }
        public string categoria { get; set; }
        public string prioridad { get; set; }
        public int? idPrioridad { get; set; }

        public List<SelectListItem> Tecnicos { get; set; }
        public List<SelectListItem> Prioridades { get; set; }

    }
}
