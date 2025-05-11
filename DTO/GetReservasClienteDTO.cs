using System;
using System.Collections.Generic;

namespace Models.DTOs
// DTO dedicado al endpoint para obtener todos los detalles de reservas de un cliente especifico por ID usuario
{

    public class GetReservasClienteDTO
    {
        public int IdReserva { get; set; }
        public DateTime FechaReserva { get; set; }
        public string DescripcionReserva { get; set; }
        public decimal PrecioTotal { get; set; }
        
        public string UsuarioNombre { get; set; }
        public string UsuarioApellidos { get; set; }
        public string UsuarioEmail { get; set; }
        
        public List<GetReservasClienteLineaDTO> Lineas { get; set; } = new List<GetReservasClienteLineaDTO>();
    }

    public class GetReservasClienteLineaDTO
    {
        public int IdLinea { get; set; }
        public decimal PrecioLinea { get; set; }
        public int NumeroAsiento { get; set; }
        public int CodigoMesa { get; set; }
        public string ImagenPuesto { get; set; }
        
        public GetReservasClienteZonaDTO Zona { get; set; }
        
        public GetReservasClienteSalaDTO Sala { get; set; }
        
        public GetReservasClienteHorarioDTO Horario { get; set; }
    }

    public class GetReservasClienteZonaDTO
    {
        public int IdZonaTrabajo { get; set; }
        public string Descripcion { get; set; }
    }

    public class GetReservasClienteSalaDTO
    {
        public int IdSala { get; set; }
        public string Nombre { get; set; }
        public string ImagenSala { get; set; }
        public string TipoSala { get; set; }
        public bool EsPrivada { get; set; }
        public string TipoPuestoTrabajo { get; set; }
        public string DescripcionPuesto { get; set; }
        public string CaracteristicasSala { get; set; }
        
        public GetReservasClienteSedeDTO Sede { get; set; }
    }

    public class GetReservasClienteSedeDTO
    {
        public int IdSede { get; set; }
        public string Pais { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Planta { get; set; }
        public string ImagenSede { get; set; }
    }

    public class GetReservasClienteHorarioDTO
    {
        public DateTime? FechaDisponibilidad { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
    }
}