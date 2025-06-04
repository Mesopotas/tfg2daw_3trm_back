using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Reservas
    {
        [Key]
        public int IdReserva { get; set; }

        public int IdUsuario { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; }

        public decimal PrecioTotal { get; set; }

        public string? TramoReservado { get; set; } // nullable en principio

        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }
        public List<Lineas> Lineas { get; set; }

        public Reservas() { }

        public Reservas(int idReserva, int idUsuario, DateTime fecha, string descripcion, decimal precioTotal)
        {
            IdReserva = idReserva;
            IdUsuario = idUsuario;
            Fecha = fecha;
            Descripcion = descripcion;
            PrecioTotal = precioTotal;
        }

        public Reservas(int idReserva, int idUsuario, DateTime fecha, string descripcion, decimal precioTotal, string tramoReservado)
        {
            IdReserva = idReserva;
            IdUsuario = idUsuario;
            Fecha = fecha;
            Descripcion = descripcion;
            PrecioTotal = precioTotal;
            TramoReservado = tramoReservado;
        }
    }
}