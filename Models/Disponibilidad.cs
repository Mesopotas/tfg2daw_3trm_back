namespace Models;

public class Disponibilidad
{
        public int IdDisponibilidad { get; set; }
        public int Fecha { get; set; }
        public bool Estado { get; set; }
        public int IdTramoHorario { get; set; }
        public int idPuestoTrabajo { get; set; }

    public Disponibilidad() { }

    public Disponibilidad(int idDisponibilidad, int fecha, bool estado, int idTramoHorario, int idPuestoTrabajo)
    {
        IdDisponibilidad = idDisponibilidad;
        Fecha = fecha;
        Estado = estado;
        IdTramoHorario = idTramoHorario;
        this.idPuestoTrabajo = idPuestoTrabajo;
    }
}