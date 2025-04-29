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
    public class LineasService : ILineasService
    {
        private readonly ILineasRepository _lineasRepository;

        public LineasService(ILineasRepository lineasRepository)
        {
            _lineasRepository = lineasRepository;
        }

        public async Task<List<Lineas>> GetAllAsync()
        {
            return await _lineasRepository.GetAllAsync();
        }

        public async Task<Lineas?> GetByIdAsync(int id)
        {
            return await _lineasRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Lineas linea)
        {
            await _lineasRepository.AddAsync(linea);
        }

        public async Task DeleteAsync(int id)
        {
           var linea = await _lineasRepository.GetByIdAsync(id);
           if (linea == null)
           {
               //return NotFound();
           }
           await _lineasRepository.DeleteAsync(id);
           //return NoContent();
        }


                }
}