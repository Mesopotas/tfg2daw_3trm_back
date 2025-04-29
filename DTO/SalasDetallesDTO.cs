namespace Models
{
    public class SalasDetallesDTO
    {
        public int IdSala { get; set; }
        public List<PuestosTrabajo> Puestos { get; set; } = new();
        public List<ZonasTrabajo> Zona { get; set; } = new();
    }
}