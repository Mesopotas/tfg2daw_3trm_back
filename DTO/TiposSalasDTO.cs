using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TiposSalasDTO
    {
        [Key]
        public int IdTipoSala { get; set; }

        public string Nombre { get; set; }

        public int NumeroMesas { get; set; }

        public int CapacidadAsientos { get; set; }

        public bool EsPrivada { get; set; }

        public string? Descripcion { get; set; }

        public int IdTipoPuestoTrabajo { get; set; }


    }
}
