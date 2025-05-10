using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

public class Lineas
{
    [Key]
    public int IdLinea { get; set; }
    public int IdReserva { get; set; }
    public double Precio { get; set; }

    [ForeignKey("IdReserva")]
    public Reservas Reserva { get; set; }

    public Lineas() { }

    public Lineas(int idLinea, int idReserva, double precio)
    {
        IdLinea = idLinea;
        IdReserva = idReserva;
        Precio = precio;
    }
}
