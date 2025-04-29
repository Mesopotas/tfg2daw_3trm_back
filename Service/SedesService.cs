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
    public class SedesService : ISedesService
    {
        private readonly ISedesRepository _sedesRepository;

        public SedesService(ISedesRepository sedesRepository)
        {
            _sedesRepository = sedesRepository;
        }

        public async Task<List<Sedes>> GetAllAsync()
        {
            return await _sedesRepository.GetAllAsync();
        }

        public async Task<Sedes?> GetByIdAsync(int id)
        {
            return await _sedesRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Sedes sede)
        {
            await _sedesRepository.AddAsync(sede);
        }

        public async Task UpdateAsync(Sedes sede)
        {
            await _sedesRepository.UpdateAsync(sede);
        }

        public async Task DeleteAsync(int id)
        {
           var sede = await _sedesRepository.GetByIdAsync(id);
           if (sede == null)
           {
               //return NotFound();
           }
           await _sedesRepository.DeleteAsync(id);
           //return NoContent();
        }


                }
}