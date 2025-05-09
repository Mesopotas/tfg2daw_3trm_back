using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Sedes
    {
        [Key]
        public int IdSede { get; set; }

        public string Pais { get; set; }

        public string Ciudad { get; set; }

        public string Direccion { get; set; }

        public string CodigoPostal { get; set; }

        public string Planta { get; set; }

        public string URL_Imagen { get; set; }

        public string Observaciones { get; set; }

        public Sedes() { } // CONTRUCTOR VACIO INYECCION DE DEPENDENCIAS

        public Sedes(int idSede, string pais, string ciudad, string direccion, string codigoPostal, string planta, string observaciones, string urlImagen)
        {
            IdSede = idSede;
            Pais = pais;
            Ciudad = ciudad;
            Direccion = direccion;
            CodigoPostal = codigoPostal;
            Planta = planta;
            Observaciones = observaciones;
            URL_Imagen = urlImagen;
        }
    }
}
