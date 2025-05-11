using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class SalaConCaracteristicas
    {
        [Key]
        public int IdSalaConCaracteristica { get; set; }

        public int IdSala { get; set; }

        public int IdCaracteristica { get; set; }

        [ForeignKey("IdSala")]
        public Salas Sala { get; set; }

        [ForeignKey("IdCaracteristica")]
        public CaracteristicasSala Caracteristica { get; set; }

        public SalaConCaracteristicas() { }

        public SalaConCaracteristicas(int idSala, int idCaracteristica)
        {
            IdSala = idSala;
            IdCaracteristica = idCaracteristica;
        }
    }
}
