using System.ComponentModel.DataAnnotations;
// para el form de la web, que nos llegue un correo con ello automatico
namespace CoWorking.DTO
{
    public class EmailFormularioContactoDTO
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos.")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La consulta es requerida.")]
        public string Consulta { get; set; }
    }
}