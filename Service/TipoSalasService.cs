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
    public class TipoSalasService : ITipoSalasService
    {
        private readonly ITipoSalasRepository _tipoSalasRepository;

        public TipoSalasService(ITipoSalasRepository tipoSalasRepository)
        {
            _tipoSalasRepository = tipoSalasRepository;
        }

        public async Task<List<TipoSalas>> GetAllAsync()
        {
            return await _tipoSalasRepository.GetAllAsync();
        }

        public async Task<TipoSalas?> GetByIdAsync(int id)
        {
            return await _tipoSalasRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TipoSalas tipoSala)
        {
            await _tipoSalasRepository.AddAsync(tipoSala);
        }

        public async Task UpdateAsync(TipoSalas tipoSala)
        {
            await _tipoSalasRepository.UpdateAsync(tipoSala);
        }

        public async Task DeleteAsync(int id)
        {
           var tipoSala = await _tipoSalasRepository.GetByIdAsync(id);
           if (tipoSala == null)
           {
               //return NotFound();
           }
           await _tipoSalasRepository.DeleteAsync(id);
           //return NoContent();
        }


                }
}