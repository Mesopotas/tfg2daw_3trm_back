using System.ComponentModel.DataAnnotations;

namespace Models
{

    // DTO dedicado al endpoint que hará el post de reservas y lineas a la vez a la hora de hacer una compra

public class ReservaPostDTO
{
    public int IdUsuario { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaReserva { get; set; }
    public List<LineasPostReservaDTO> Lineas { get; set; }
}

public class LineasPostReservaDTO
{
    public int IdPuestoTrabajo { get; set; }
    public int IdTramoHorario { get; set; }
}

}