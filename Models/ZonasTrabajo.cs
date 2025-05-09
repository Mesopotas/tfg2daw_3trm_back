using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ZonasTrabajo
    {
        [Key]
        public int IdZonaTrabajo { get; set; }

        public string Descripcion { get; set; }

        public int IdSala { get; set; }

        [ForeignKey("IdSala")]
        public Salas Sala { get; set; }

        public ZonasTrabajo() { }

        public ZonasTrabajo(int idZonaTrabajo, string descripcion, int idSala)
        {
            IdZonaTrabajo = idZonaTrabajo;
            Descripcion = descripcion;
            IdSala = idSala;
        }
    }
}
