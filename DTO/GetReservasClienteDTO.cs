using System;
using System.Collections.Generic;

namespace Models.DTOs
{
    public class GetReservasClienteDTO
    {
        public int IdReserva { get; set; }
        public decimal PrecioTotal { get; set; }

        public string? NombreSalaPrincipal { get; set; } 
        public string? CiudadSedePrincipal { get; set; }
        public string? DireccionSedePrincipal { get; set; }
        public string? ImagenSalaPrincipal { get; set; }

        public string? RangoHorarioReserva { get; set; } // string ya que se devolverá formateado
        public int CantidadHorasReservadas { get; set; } // conteo de disponibilidades de la reserva
    }
}