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

        public int IdPuestoTrabajo { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; }

        public double PrecioTotal { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }

        public Reservas() { }

        public Reservas(int idReserva, int idUsuario, DateTime fecha, string descripcion, double precioTotal, int idPuestoTrabajo)
        {
            IdReserva = idReserva;
            IdUsuario = idUsuario;
            Fecha = fecha;
            Descripcion = descripcion;
            PrecioTotal = precioTotal;
            IdPuestoTrabajo = idPuestoTrabajo;
        }
    }
}
