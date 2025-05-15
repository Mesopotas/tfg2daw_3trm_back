    public class SalasDTO
    {
        public int IdSala { get; set; }
        public string Nombre { get; set; }
        public string URL_Imagen { get; set; }
        public int Capacidad { get; set; }
        public int IdTipoSala { get; set; }
        public int IdSede { get; set; }
        public bool Bloqueado { get; set; }
    }

public class SalasFiltradoDTO
{
    public int IdSala { get; set; }
    public string Nombre { get; set; }
    public string URL_Imagen { get; set; }
    public int Capacidad { get; set; }
    public int IdTipoSala { get; set; }
    public int IdSede { get; set; }

    public string SedeObservaciones { get; set; }
    public string SedePlanta { get; set; }
    public string SedeDireccion { get; set; }
    public string SedeCiudad { get; set; }   

    public int PuestosDisponibles { get; set; }
    public int PuestosOcupados { get; set; }

 }

