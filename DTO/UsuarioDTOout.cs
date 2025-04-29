using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UserDTOOut
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Rol { get; set; }
    }
}