using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace Models;

public class Sedes{

    [Key]
    public int IdSede{get; set;}
    public string  Pais  {get; set;}
    public string  Ciudad {get; set;}
    public string  Direccion {get; set;}
    public string  CodigoPostal {get; set;}
    public string  Planta  {get; set;}
   public string URL_Imagen { get; set; } 
    public string Latitud { get; set; } 
    public string Longitud { get; set; }
    public string Observaciones { get; set; }
    public Sedes(){} // CONTRUCTOR VACIO INYECCION DE DEPENDENCIAS

    public Sedes(int idSede, string pais, string ciudad, string direccion, string codigoPostal, string planta, string url_Imagen, string latitud, string longitud, string observaciones){

        IdSede = idSede;
        Pais = pais;
        Ciudad = ciudad;
        Direccion = direccion;
        CodigoPostal = codigoPostal;
        Planta = planta;
        URL_Imagen = url_Imagen;
        Latitud = latitud;
        Longitud = longitud;
        Observaciones = observaciones;
    }


}