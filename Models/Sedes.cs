using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace Models;

public class Sedes{

    public int IdSede{get; set;}
    public string  Pais  {get; set;}
    public string  Ciudad {get; set;}
    public string  Direccion {get; set;}
    public string  CodigoPostal {get; set;}
    public string  Planta  {get; set;}
    public string  Observaciones  {get; set;}
    public Sedes(){} // CONTRUCTOR VACIO INYECCION DE DEPENDENCIAS

    public Sedes(int idSede, string pais, string ciudad, string direccion, string codigoPostal, string planta, string observaciones){

        IdSede = idSede;
        Pais = pais;
        Ciudad = ciudad;
        Direccion = direccion;
        CodigoPostal = codigoPostal;
        Planta = planta;
        Observaciones = observaciones;
    }


}