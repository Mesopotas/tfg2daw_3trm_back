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
    public class RolesService : IRolesService
    {
        private readonly IRolesRepository _rolesRepository;

        public RolesService(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        public async Task<List<Roles>> GetAllAsync()
        {
            return await _rolesRepository.GetAllAsync();
        }

        public async Task<Roles?> GetByIdAsync(int id)
        {
            return await _rolesRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Roles rol)
        {
            await _rolesRepository.AddAsync(rol);
        }

        public async Task UpdateAsync(Roles rol)
        {
            await _rolesRepository.UpdateAsync(rol);
        }

        public async Task DeleteAsync(int id)
        {
           var rol = await _rolesRepository.GetByIdAsync(id);
           if (rol == null)
           {
               //return NotFound();
           }
           await _rolesRepository.DeleteAsync(id);
           //return NoContent();
        }


                }
}