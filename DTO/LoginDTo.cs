using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models;

public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Contrasenia { get; set; }
}