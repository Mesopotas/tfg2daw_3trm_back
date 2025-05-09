using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class TramosHorarios
    {
        [Key]
        public int IdTramoHorario { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public TramosHorarios() { }

        public TramosHorarios(int idTramoHorario, TimeSpan horaInicio, TimeSpan horaFin)
        {
            IdTramoHorario = idTramoHorario;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
        }
    }
}
