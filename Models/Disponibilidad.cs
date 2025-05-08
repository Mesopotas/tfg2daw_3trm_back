using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Disponibilidad
    {
        [Key]
        public int IdDisponibilidad { get; set; }

        public DateTime Fecha { get; set; }

        public bool Estado { get; set; } = true;

        [ForeignKey(nameof(TramoHorario))]
        public int IdTramoHorario { get; set; }

        [ForeignKey(nameof(PuestoTrabajo))]
        public int IdPuestoTrabajo { get; set; }

        // para los joins
        public TramosHorarios TramoHorario { get; set; }
        public PuestosTrabajo PuestoTrabajo { get; set; }

        public Disponibilidad() { }

        public Disponibilidad(int idDisponibilidad, DateTime fecha, bool estado, int idTramoHorario, int idPuestoTrabajo)
        {
            IdDisponibilidad = idDisponibilidad;
            Fecha = fecha;
            Estado = estado;
            IdTramoHorario = idTramoHorario;
            IdPuestoTrabajo = idPuestoTrabajo;
        }
    }
}