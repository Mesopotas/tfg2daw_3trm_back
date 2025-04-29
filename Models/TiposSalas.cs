namespace Models
{
    public class TipoSalas
    {
        public int IdTipoSala { get; set; }
        public string Nombre { get; set; }
        public int NumeroMesas { get; set; }
        public int CapacidadAsientos { get; set; }
        public bool EsPrivada { get; set; }
        public string? Descripcion { get; set; }
        public int IdTipoPuestoTrabajo { get; set; }

        public TipoSalas() { }

        public TipoSalas(int idTipoSala, string nombre, int numeroMesas, int capacidadAsientos, bool esPrivada, string? descripcion, int idTipoPuestoTrabajo)
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