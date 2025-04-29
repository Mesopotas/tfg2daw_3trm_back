using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        
        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        public string Apellidos { get; set; }

        [EmailAddress(ErrorMessage = "El email no es válido")]
        [Required(ErrorMessage = "El email es obligatorio")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 30 caracteres")]
        public string Contrasenia { get; set; }
    }
}
