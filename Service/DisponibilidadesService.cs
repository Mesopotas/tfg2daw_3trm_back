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
    public class DisponibilidadesService : IDisponibilidadesService
    {
        private readonly IDisponibilidadesRepository _disponibilidadesRepository;

        public DisponibilidadesService(IDisponibilidadesRepository disponibilidadesRepository)
        {
            _disponibilidadesRepository = disponibilidadesRepository;
        }

        public async Task<List<DisponibilidadDTO>> GetAllAsync()
        {
            return await _disponibilidadesRepository.GetAllAsync();
        }

        public async Task<DisponibilidadDTO> GetByIdAsync(int id)
        {
            return await _disponibilidadesRepository.GetByIdAsync(id);
        }

        public async Task<List<DisponibilidadDTO>> GetByIdPuestoTrabajoAsync(int id)
        {
            return await _disponibilidadesRepository.GetByIdPuestoTrabajoAsync(id);
        }


       public async Task UpdateDisponibilidadAsync(DisponibilidadDTO disponibilidad)
        {
            await _disponibilidadesRepository.UpdateDisponibilidadAsync(disponibilidad);
        }

          public async Task AddDisponibilidadesAsync(int anio)
        {
            await _disponibilidadesRepository.AddDisponibilidadesAsync(anio);
        }
        public async Task<List<FechasDisponiblesDTO>> GetDiasBySalaAsync(int salaId)
        {
            return await _disponibilidadesRepository.GetDiasBySalaAsync(salaId);
        }
   

    }
}