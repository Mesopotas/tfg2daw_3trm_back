using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Models;

public class TiposPuestosTrabajo
{
    [Key]
        public int IdTipoPuestoTrabajo { get; set; }
        public string Nombre { get; set; }
        public string Imagen_URL { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        
    public TiposPuestosTrabajo() { }

    public TiposPuestosTrabajo(int idTipoPuestoTrabajo, string nombre, string imagen_URL, string descripcion, decimal precio)
    {
        IdTipoPuestoTrabajo = idTipoPuestoTrabajo;
        Nombre = nombre;
        Imagen_URL= imagen_URL;
        Descripcion= descripcion;
        Precio= precio;

    }
}