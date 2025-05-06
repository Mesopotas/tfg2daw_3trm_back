namespace Models;

public class Lineas
{
    public int IdLinea { get; set; }
    public int IdReserva { get; set; }
    public double Precio { get; set; }


    public Lineas() { }

    public Lineas(int idLinea, int idReserva, double precio)
    {
        IdLinea = idLinea;
        IdReserva = idReserva;
        Precio = precio;
    }
}
