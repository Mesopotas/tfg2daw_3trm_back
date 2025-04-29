using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoWorking.Repositories;
using CoWorking.DTO;
using CoWorking.Service;

namespace CoWorking.Service
{
    public class DetallesReservasService : IDetallesReservasService
    {
        private readonly IDetallesReservasRepository _detallesReservasRepository;

        public DetallesReservasService(IDetallesReservasRepository detallesReservasRepository)
        {
            _detallesReservasRepository = detallesReservasRepository;
        }

        public async Task<List<DetallesReservas>> GetAllAsync()
        {
            return await _detallesReservasRepository.GetAllAsync();
        }

        public async Task<DetallesReservas?> GetByIdAsync(int id)
        {
            return await _detallesReservasRepository.GetByIdAsync(id);
        }

        public async Task<int?> GetLastIdAsync()
        {
            return await _detallesReservasRepository.GetLastIdAsync();
        }


        public async Task AddAsync(DetallesReservas detalleReserva)
        {
            await _detallesReservasRepository.AddAsync(detalleReserva);
        }

        public async Task UpdateAsync(DetallesReservas detalleReserva)
        {
            await _detallesReservasRepository.UpdateAsync(detalleReserva);
        }

        public async Task DeleteAsync(int id)
        {
           var detalleReserva = await _detallesReservasRepository.GetByIdAsync(id);
           if (detalleReserva == null)
           {
               //return NotFound();
           }
           await _detallesReservasRepository.DeleteAsync(id);
           //return NoContent();
        }


                }
}