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
    public class TiposSalasService : ITiposSalasService
    {
        private readonly ITiposSalasRepository _TiposSalasRepository;

        public TiposSalasService(ITiposSalasRepository TiposSalasRepository)
        {
            _TiposSalasRepository = TiposSalasRepository;
        }

        public async Task<List<TiposSalas>> GetAllAsync()
        {
            return await _TiposSalasRepository.GetAllAsync();
        }

        public async Task<TiposSalas?> GetByIdAsync(int id)
        {
            return await _TiposSalasRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TiposSalasDTO tipoSala)
        {
            await _TiposSalasRepository.AddAsync(tipoSala);
        }

        public async Task UpdateAsync(TiposSalas tipoSala)
        {
            await _TiposSalasRepository.UpdateAsync(tipoSala);
        }

        public async Task DeleteAsync(int id)
        {
           var tipoSala = await _TiposSalasRepository.GetByIdAsync(id);
           if (tipoSala == null)
           {
               //return NotFound();
           }
           await _TiposSalasRepository.DeleteAsync(id);
           //return NoContent();
        }


                }
}