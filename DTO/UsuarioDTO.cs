using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UsuarioUpdateDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public int IdRol { get; set; }
    }

}
