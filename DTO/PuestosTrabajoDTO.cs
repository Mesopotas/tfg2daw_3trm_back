namespace Dtos
{
    public class PuestosTrabajoDTO // sin llamar a los objetos de las relaciones
    {
        public int IdPuestoTrabajo { get; set; }
        public int NumeroAsiento { get; set; }
        public int CodigoMesa { get; set; }
        public string URL_Imagen { get; set; }
        public bool Disponible { get; set; }
        public bool Bloqueado { get; set; }
        public int IdZonaTrabajo { get; set; }
        public int IdSala { get; set; }
    }
}
