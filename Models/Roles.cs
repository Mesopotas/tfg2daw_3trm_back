using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace Models;

public class Roles{

    [Key]
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