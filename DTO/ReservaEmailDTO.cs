namespace CoWorking.Models.DTO
{
    // contenido del email de confirmacion de reserva
    public class ReservationEmailData
    {
        public int IdReserva { get; set; }
        public DateTime Fecha { get; set; }
        public decimal PrecioTotal { get; set; }
        public string? NombreSala { get; set; }
        public string? CiudadSede { get; set; }
        public string? DireccionSede { get; set; }
        public string? RangoHorario { get; set; }
        public string? AsientosReservados { get; set; }
    }
}