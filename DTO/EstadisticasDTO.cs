// CoWorking.DTO/EstadisticasDTOs.cs

namespace CoWorking.DTO
{
    public class DashboardEstadisticasDTO
    {
        // estadisticas generales
        public int TotalUsuarios { get; set; }
        public int TotalReservas { get; set; }
        public int ReservasMesActual { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal IngresosMesActual { get; set; }
        
        // graficas
        public List<ReservasPorSedeDTO> ReservasPorSede { get; set; } = new();
        public List<ReservasPorMesDTO> ReservasPorMes { get; set; } = new();
        public List<TipoSalaReservasDTO> TiposSalaMasReservados { get; set; } = new();
        public List<OcupacionDiaDTO> OcupacionPorDia { get; set; } = new();
        public List<UsuarioActivoDTO> UsuariosMasActivos { get; set; } = new();
    }

    public class ReservasPorSedeDTO
    {
        public string Ciudad { get; set; } = string.Empty;
        public int CantidadReservas { get; set; }
        public decimal Ingresos { get; set; }
    }

    public class ReservasPorMesDTO
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public int CantidadReservas { get; set; }
        public decimal Ingresos { get; set; }
        public string MesNombre => GetNombreMes(Mes);
        public string FechaFormateada => $"{MesNombre} {Año}";
        
        private string GetNombreMes(int mes)
        {
            return mes switch
            {
                1 => "Enero", 2 => "Febrero", 3 => "Marzo", 4 => "Abril",
                5 => "Mayo", 6 => "Junio", 7 => "Julio", 8 => "Agosto",
                9 => "Septiembre", 10 => "Octubre", 11 => "Noviembre", 12 => "Diciembre",
                _ => "N/A"
            };
        }
    }

    public class TipoSalaReservasDTO
    {
        public string TipoSala { get; set; } = string.Empty;
        public int CantidadReservas { get; set; }
        public decimal Ingresos { get; set; }
    }

    public class OcupacionDiaDTO
    {
        public int DiaSemana { get; set; }
        public int HorasOcupadas { get; set; }
        public string NombreDia => GetNombreDia(DiaSemana); // funcion para autoasignar los nombres, para q la api los saque ya convertidos
        
        private string GetNombreDia(int dia)
        {
            return dia switch
            {
                0 => "Domingo", 1 => "Lunes", 2 => "Martes", 3 => "Miércoles",
                4 => "Jueves", 5 => "Viernes", 6 => "Sábado",
                _ => "N/A"
            };
        }
    }

    public class UsuarioActivoDTO
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int CantidadReservas { get; set; }
        public decimal TotalGastado { get; set; }
    }

    public class EvolucionIngresosDTO
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public decimal Ingresos { get; set; }
        public int CantidadReservas { get; set; }
        public string MesNombre => GetNombreMes(Mes);
        public string FechaFormateada => $"{MesNombre} {Año}";
        
        private string GetNombreMes(int mes)
        {
            return mes switch
            {
                1 => "Enero", 2 => "Febrero", 3 => "Marzo", 4 => "Abril",
                5 => "Mayo", 6 => "Junio", 7 => "Julio", 8 => "Agosto",
                9 => "Septiembre", 10 => "Octubre", 11 => "Noviembre", 12 => "Diciembre",
                _ => "N/A"
            };
        }
    }

    public class EstadisticasSalaDTO
    {
        public string NombreSala { get; set; } = string.Empty;
        public string TipoSala { get; set; } = string.Empty;
        public int CapacidadTotal { get; set; }
        public int VecesReservada { get; set; }
        public decimal IngresosTotales { get; set; }
        public double TasaOcupacion { get; set; }
        public string TasaOcupacionFormateada => $"{TasaOcupacion:F1}%";
    }
}