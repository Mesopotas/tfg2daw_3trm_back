using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace Models;

public class Roles{

    public int IdRol{get; set;}
    public string  Nombre  {get; set;}
    public string  Descripcion {get; set;}
    public Roles(){} // CONTRUCTOR VACIO INYECCION DE DEPENDENCIAS

    public Roles(int idRol, string nombre, string descripcion){

        IdRol = idRol;
        Nombre = nombre;
        Descripcion = descripcion;
    }


}