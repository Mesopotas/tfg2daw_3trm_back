namespace Models;

public class DetallesReservas
{
    public int IdDetalleReserva { get; set; }
    public string Descripcion { get; set; }
    public int IdPuestoTrabajo { get; set; }

    public DetallesReservas() { }

    public DetallesReservas(int idDetalleReserva, string descripcion, int idPuestoTrabajo)
    {
        IdDetalleReserva = idDetalleReserva;
        Descripcion = descripcion;
        IdPuestoTrabajo = idPuestoTrabajo;
    }
}
