
public class PuestoTrabajoFiltroFechasDTO
{
    public int IdPuestoTrabajo { get; set; }
    public int? NumeroAsiento { get; set; }
    public int? CodigoMesa { get; set; }
    public string URL_Imagen { get; set; }
    public bool DisponibleGeneral { get; set; }
    public bool BloqueadoGeneral { get; set; }
    public int? IdZonaTrabajo { get; set; }
    public int IdTipoPuestoTrabajo { get; set; }
    public int IdSala { get; set; }


    public List<DisponibilidadFiltroFechasDTO> DisponibilidadesEnRango { get; set; }
}

public class DisponibilidadFiltroFechasDTO
{
    public int IdDisponibilidad { get; set; }
    public DateTime Fecha { get; set; }
    public bool Estado { get; set; }
    public int IdTramoHorario { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
}
