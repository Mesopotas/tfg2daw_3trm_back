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
    public class TramosHorariosService : ITramosHorariosService
    {
        private readonly ITramosHorariosRepository _tramosHorariosRepository;

        public TramosHorariosService(ITramosHorariosRepository tramosHorariosRepository)
        {
            _tramosHorariosRepository = tramosHorariosRepository;
        }

        public async Task<List<TramosHorarios>> GetAllAsync()
        {
            return await _tramosHorariosRepository.GetAllAsync();
        }

        public async Task<TramosHorarios?> GetByIdAsync(int id)
        {
            return await _tramosHorariosRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TramosHorarios tramoHorario)
        {
            await _tramosHorariosRepository.AddAsync(tramoHorario);
        }

        public async Task UpdateAsync(TramosHorarios tramoHorario)
        {
            await _tramosHorariosRepository.UpdateAsync(tramoHorario);
        }

        public async Task DeleteAsync(int id)
        {
            var tramoHorario = await _tramosHorariosRepository.GetByIdAsync(id);
            if (tramoHorario == null)
            {
                //return NotFound();
            }
            await _tramosHorariosRepository.DeleteAsync(id);
            //return NoContent();
        }


    }
}