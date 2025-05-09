using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class PuestosTrabajo
    {
        [Key]
        public int IdPuestoTrabajo { get; set; }

        public int NumeroAsiento { get; set; }
        public int CodigoMesa { get; set; }
        public string URL_Imagen { get; set; }
        public bool Disponible { get; set; }
        public bool Bloqueado { get; set; }

        public int IdZonaTrabajo { get; set; }
        public int IdSala { get; set; }

        [ForeignKey("IdZonaTrabajo")]
        public ZonasTrabajo ZonaTrabajo { get; set; }

        [ForeignKey("IdSala")]
        public Salas Sala { get; set; }

        public PuestosTrabajo() { }

        public PuestosTrabajo(int idPuestoTrabajo, int numeroAsiento, int codigoMesa, string urlImagen, bool disponible, bool bloqueado, int idZonaTrabajo, int idSala)
        {
            IdPuestoTrabajo = idPuestoTrabajo;
            NumeroAsiento = numeroAsiento;
            CodigoMesa = codigoMesa;
            URL_Imagen = urlImagen;
            Disponible = disponible;
            Bloqueado = bloqueado;
            IdZonaTrabajo = idZonaTrabajo;
            IdSala = idSala;
        }
    }
}
