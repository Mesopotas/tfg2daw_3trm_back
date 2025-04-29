namespace Models;

public class TramosHorarios{

    public int IdTramoHorario{get; set;}
    public string HoraInicio  {get; set;}
    public string HoraFin  {get; set;}
    public int DiaSemanal  {get; set;}
    public TramosHorarios(){} // CONTRUCTOR VACIO INYECCION DE DEPENDENCIAS

    public TramosHorarios(int idTramoHorario, string horaInicio,string horaFin , int diaSemanal){

        IdTramoHorario = idTramoHorario;
        HoraInicio = horaInicio;
        HoraFin = horaFin;
        DiaSemanal = diaSemanal;
    }


}