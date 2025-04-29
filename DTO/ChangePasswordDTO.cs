using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ChangePasswordDTO
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "La contraseña es demasiado corta")] // 255 es el espacio asignado en bbdd
        public string NewPassword { get; set; }
    }
}
