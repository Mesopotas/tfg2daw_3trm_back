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
    public class SalasService : ISalasService
    {
        private readonly ISalasRepository _salasRepository;

        public SalasService(ISalasRepository salasRepository)
        {
            _salasRepository = salasRepository;
        }

        public async Task<List<SalasDTO>> GetAllAsync()
        {
            return await _salasRepository.GetAllAsync();
        }

        public async Task<SalasDTO> GetByIdAsync(int id)
        {
            return await _salasRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(SalasDTO sala)
        {
            await _salasRepository.AddAsync(sala);
        }

        public async Task UpdateAsync(SalasDTO sala)
        {
            await _salasRepository.UpdateAsync(sala);
        }

        public async Task DeleteAsync(int id)
        {
            var sala = await _salasRepository.GetByIdAsync(id);
            if (sala == null)
            {
                //return NotFound();
            }
            await _salasRepository.DeleteAsync(id);
            //return NoContent();
        }
        
     public async Task<List<SalasDTO>> GetByIdSedeAsync(int idSede)
        {
            return await _salasRepository.GetByIdSedeAsync(idSede);
        }
        
    public async Task<List<SalasFiltradoDTO>> GetSalasBySede(int idSede, DateTime fechaInicio, DateTime fechaFin, TimeSpan horaInicio, TimeSpan horaFin)
        {
            return await _salasRepository.GetSalasBySede(idSede, fechaInicio, fechaFin, horaInicio, horaFin);
        }
    }
}