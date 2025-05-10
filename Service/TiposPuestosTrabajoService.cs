using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoWorking.Repositories;
using CoWorking.DTO;
using CoWorking.Service;
using System.Security.Claims;

namespace CoWorking.Service
{
    public class TiposPuestosTrabajoService : ITiposPuestosTrabajoService
    {
        private readonly ITiposPuestosTrabajoRepository _tiposPuestosTrabajoRepository;

        public TiposPuestosTrabajoService(ITiposPuestosTrabajoRepository tiposPuestosTrabajoRepository)
        {
            _tiposPuestosTrabajoRepository = tiposPuestosTrabajoRepository;
        }

        public async Task<List<TiposPuestosTrabajo>> GetAllAsync()
        {
            return await _tiposPuestosTrabajoRepository.GetAllAsync();
        }

        public async Task<TiposPuestosTrabajo?> GetByIdAsync(int id)
        {
            return await _tiposPuestosTrabajoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TiposPuestosTrabajo tipoPuestoTrabajo)
        {
            await _tiposPuestosTrabajoRepository.AddAsync(tipoPuestoTrabajo);
        }

        public async Task UpdateAsync(TiposPuestosTrabajo tipoPuestoTrabajo)
        {
            await _tiposPuestosTrabajoRepository.UpdateAsync(tipoPuestoTrabajo);
        }

        public async Task DeleteAsync(int id)
        {
            var tipoPuestoTrabajo = await _tiposPuestosTrabajoRepository.GetByIdAsync(id);
            if (tipoPuestoTrabajo == null)
            {
                //return NotFound();
            }
            await _tiposPuestosTrabajoRepository.DeleteAsync(id);
            //return NoContent();
        }
    }
}