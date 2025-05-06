using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Models;

public class Usuarios{
    [Key]
    public int IdUsuario{get; set;}
    public string  Nombre  {get; set;}
    public string Apellidos {get; set;}
    public string Email  {get; set;}
    public string Contrasenia  {get; set;}
    public DateTime FechaRegistro  {get; set;}
    public int IdRol  {get; set;}

    [ForeignKey("IdRol")]
    public Roles Rol { get; set; } // relacion a la tabla roles para hacer joins
    public Usuarios(){} // CONTRUCTOR VACIO INYECCION DE DEPENDENCIAS

    public Usuarios(int idUsuario, string nombre, string apellidos, string email, string contrasenia, DateTime fechaRegistro, int idRol){

        IdUsuario = idUsuario;
        Nombre = nombre;
        Apellidos = apellidos;
        Email = email;
        Contrasenia = contrasenia;
        FechaRegistro = fechaRegistro;
        IdRol = idRol;

    }


}