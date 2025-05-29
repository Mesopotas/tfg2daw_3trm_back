using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoWorking.Repositories;
using CoWorking.DTO;
using CoWorking.Data;
using CoWorking.Service;
using Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CoWorking.Service
{
    public class ReservasService : IReservasService
    {
        private readonly IReservasRepository _reservasRepository;
        private readonly IEmailService _emailService;
        private readonly CoworkingDBContext _context;

        public ReservasService(IReservasRepository reservasRepository, IEmailService emailService, CoworkingDBContext context, ILogger<ReservasService> logger)
        {
            _reservasRepository = reservasRepository;
            _emailService = emailService;
            _context = context;
        }

        public async Task<List<ReservasDTO>> GetAllAsync()
        {
            return await _reservasRepository.GetAllAsync();
        }

        public async Task<ReservasDTO> GetByIdAsync(int id)
        {
            return await _reservasRepository.GetByIdAsync(id);
        }

        public async Task CreateReservaAsync(Reservas reserva)
        {
            await _reservasRepository.CreateReservaAsync(reserva);
        }

        public async Task UpdateAsync(ReservasUpdateDTO reserva)
        {
            await _reservasRepository.UpdateAsync(reserva);
        }

        public async Task DeleteAsync(int id)
        {
            var reserva = await _reservasRepository.GetByIdAsync(id);
            if (reserva == null)
            {
                //return NotFound();
            }
            await _reservasRepository.DeleteAsync(id);
            //return NoContent();
        }


      public async Task<GetDetallesReservaDTO> GetResumenReservaAsync(int id)
        {
            return await _reservasRepository.GetResumenReservaAsync(id);
        }


        public async Task<List<GetReservasClienteDTO>> GetReservasUsuario(int idUsuario)
        {
            return await _reservasRepository.GetReservasUsuarioAsync(idUsuario);
        }

        public async Task<Reservas> CreateReservaConLineasAsync(ReservaPostDTO reservaDTO)
        {
            // crear la reserva
            var reserva = await _reservasRepository.CreateReservaConLineasAsync(reservaDTO);

            // mandar el email de confirmación
            try
            {
                // obtener el usuario por IdUsuario
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdUsuario == reservaDTO.IdUsuario);

                if (usuario != null) // siempre habra un id usuario asociado a la reserva
                {
                    var detallesReserva = await _reservasRepository.GetResumenReservaAsync(reserva.IdReserva); // obtener la info de la reserva para rellenar el email

                    var emailData = new Models.DTO.ReservationEmailData
                    {
                        IdReserva = reserva.IdReserva,
                        Fecha = reserva.Fecha,
                        PrecioTotal = reserva.PrecioTotal,
                        NombreSala = detallesReserva?.NombreSalaPrincipal,
                        CiudadSede = detallesReserva?.CiudadSedePrincipal,
                        DireccionSede = detallesReserva?.DireccionSedePrincipal,
                        RangoHorario = detallesReserva?.RangoHorarioReserva,
                        AsientosReservados = detallesReserva?.AsientosReservados
                    };

                    await _emailService.SendReservationConfirmationAsync(
                        usuario.Email,
                        usuario.Nombre,
                        emailData
                    );
                }
            }
            catch (Exception ex)
            {
                return null;  
            }

            return reserva;
        }

        public async Task<bool> ValidarReservaExisteQR(int idReserva, int idUsuario, DateTime fecha)
                {
                    return await _reservasRepository.ValidarReservaExisteQR(idReserva, idUsuario, fecha);
                }

    }
}