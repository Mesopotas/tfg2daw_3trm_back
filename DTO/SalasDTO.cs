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
    public List<CaracteristicaSalaArrayDTO> Caracteristicas { get; set; } // este a su vez es otro dto, para que el array list de cada sala solo tenga los 2 campos que hacen falta


 }

public class CaracteristicaSalaArrayDTO
{
    public int IdCaracteristica { get; set; }
    public string Nombre { get; set; }
}
