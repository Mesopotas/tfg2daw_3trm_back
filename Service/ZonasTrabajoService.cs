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
    public class ZonasTrabajoService : IZonasTrabajoService
    {
        private readonly IZonasTrabajoRepository _zonasTrabajoRepository;

        public ZonasTrabajoService(IZonasTrabajoRepository zonasTrabajoRepository)
        {
            _zonasTrabajoRepository = zonasTrabajoRepository;
        }

        public async Task<List<ZonasTrabajo>> GetAllAsync()
        {
            return await _zonasTrabajoRepository.GetAllAsync();
        }

        public async Task<ZonasTrabajo> GetByIdAsync(int id)
        {
            return await _zonasTrabajoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(ZonasTrabajoDTO zonaTrabajoDTO)
        {
            await _zonasTrabajoRepository.AddAsync(zonaTrabajoDTO);
        }

        public async Task UpdateAsync(ZonasTrabajo zonaTrabajo)
        {
            await _zonasTrabajoRepository.UpdateAsync(zonaTrabajo);
        }

        public async Task DeleteAsync(int id)
        {
            var zonaTrabajo = await _zonasTrabajoRepository.GetByIdAsync(id);
            if (zonaTrabajo == null)
            {
                //return NotFound();
            }
            await _zonasTrabajoRepository.DeleteAsync(id);
            //return NoContent();
        }


    }
}