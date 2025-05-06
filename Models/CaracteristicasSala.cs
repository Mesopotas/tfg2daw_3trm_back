using System.ComponentModel.DataAnnotations;

namespace Models;


public class CaracteristicasSala
{
    [Key]
    public int IdCaracteristica { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public double PrecioAniadido { get; set; } = 0; //Ponemos que sea cero porque algunas salas pueden  tener caracteristicas que no tengan precio añadido 



    public CaracteristicasSala() { }

    public CaracteristicasSala(int idCaracteristica, string nombre, string descripcion, double precioAniadido)
    {
        IdCaracteristica = idCaracteristica;
        Nombre = nombre;
        Descripcion = descripcion;
        PrecioAniadido = precioAniadido;
    }
}
