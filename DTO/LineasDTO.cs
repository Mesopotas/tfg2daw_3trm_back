namespace CoWorking.DTO
{
        // dto basico para lineas
    public class LineasDTO
    {
        public int IdLinea { get; set; }
        public int IdReserva { get; set; }
        public int IdPuestoTrabajo { get; set; }
        public decimal Precio { get; set; }
    }
}
