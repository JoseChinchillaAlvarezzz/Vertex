using System.ComponentModel.DataAnnotations;

namespace Vertex.Models
{
    public class clientes
    {
        [Key]
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; }
        public string telefono {  get; set; }
        public string contrasenia { get; set; }
        public string empresa { get; set; }
        public string contactoprincipal { get; set; }
        public string nombrecontacto { get; set; }
    }
}
