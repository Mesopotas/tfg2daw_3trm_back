// DTO para la pagina de admin, para ver/editar/añadir características de salas

public class SalasConCaracteristicasDTO
{
    public int IdSala { get; set; }
    public string Nombre { get; set; }
    public string URL_Imagen { get; set; }
    public int Capacidad { get; set; }
    public int IdTipoSala { get; set; }
    public int IdSede { get; set; }
    public bool Bloqueado { get; set; }
    public List<CaracteristicaSalaDTO> Caracteristicas { get; set; } = new List<CaracteristicaSalaDTO>();
}

public class CaracteristicaSalaDTO
{
    public int IdCaracteristica { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal PrecioAniadido { get; set; }
}

// DTO para las requests de agregar/quitar características
public class SalaCaracteristicaRequestDTO
{
    public int IdSala { get; set; }
    public int IdCaracteristica { get; set; }
}