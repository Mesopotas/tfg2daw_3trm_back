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
    public class CaracteristicasSalaService : ICaracteristicasSalaService
    {
        private readonly ICaracteristicasSalaRepository _caracteristicasSalaRepository;

        public CaracteristicasSalaService(ICaracteristicasSalaRepository caracteristicasSalaRepository)
        {
            _caracteristicasSalaRepository = caracteristicasSalaRepository;
        }

        public async Task<List<CaracteristicasSala>> GetAllAsync()
        {
            return await _caracteristicasSalaRepository.GetAllAsync();
        }

        public async Task<CaracteristicasSala?> GetByIdAsync(int id)
        {
            return await _caracteristicasSalaRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(CaracteristicasSala caracteristicaSala)
        {
            await _caracteristicasSalaRepository.AddAsync(caracteristicaSala);
        }

        public async Task UpdateAsync(CaracteristicasSala caracteristicaSala)
        {
            await _caracteristicasSalaRepository.UpdateAsync(caracteristicaSala);
        }

        public async Task DeleteAsync(int id)
        {
            var caracteristicaSala = await _caracteristicasSalaRepository.GetByIdAsync(id);
            if (caracteristicaSala == null)
            {
                //return NotFound();
            }
            await _caracteristicasSalaRepository.DeleteAsync(id);
            //return NoContent();
        }
    }
}