using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TiposSalas
    {
        [Key]
        public int IdTipoSala { get; set; }

        public string Nombre { get; set; }

        public int NumeroMesas { get; set; }

        public int CapacidadAsientos { get; set; }

        public bool EsPrivada { get; set; }

        public string? Descripcion { get; set; }

        [ForeignKey("IdTipoPuestoTrabajo")] // no sirve solo con esto, habrá que ponerlo en el ContextDB definida la relacion
        public int IdTipoPuestoTrabajo { get; set; }

        public TiposPuestosTrabajo TipoPuestoTrabajo { get; set; }

        public TiposSalas() { }

        public TiposSalas(int idTipoSala, string nombre, int numeroMesas, int capacidadAsientos, bool esPrivada, string? descripcion, int idTipoPuestoTrabajo)
        {
            IdTipoSala = idTipoSala;
            Nombre = nombre;
            NumeroMesas = numeroMesas;
            CapacidadAsientos = capacidadAsientos;
            EsPrivada = esPrivada;
            Descripcion = descripcion;
            IdTipoPuestoTrabajo = idTipoPuestoTrabajo;
        }
    }
}
