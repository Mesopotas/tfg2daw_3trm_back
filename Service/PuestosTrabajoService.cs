using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoWorking.Repositories;
using CoWorking.DTO;
using Dtos;

namespace CoWorking.Service
{
    public class PuestosTrabajoService : IPuestosTrabajoService
    {
        private readonly IPuestosTrabajoRepository _puestosTrabajoRepository;

        public PuestosTrabajoService(IPuestosTrabajoRepository puestosTrabajoRepository)
        {
            _puestosTrabajoRepository = puestosTrabajoRepository;
        }

        public async Task<List<PuestosTrabajoDTO>> GetAllAsync()
        {
            return await _puestosTrabajoRepository.GetAllAsync();
        }

        public async Task<PuestosTrabajoDTO?> GetByIdAsync(int id)
        {
            return await _puestosTrabajoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(PuestosTrabajoDTO puesto)
        {
            await _puestosTrabajoRepository.AddAsync(puesto);
        }

        public async Task UpdateAsync(PuestosTrabajoDTO puesto)
        {
            await _puestosTrabajoRepository.UpdateAsync(puesto);
        }

        public async Task DeleteAsync(int id)
        {
            var puesto = await _puestosTrabajoRepository.GetByIdAsync(id);
            if (puesto == null)
            {
                throw new KeyNotFoundException("No se encontro el puesto de trabajo con ese ID");
            }
            await _puestosTrabajoRepository.DeleteAsync(id);
        }


          public async Task<List<PuestosTrabajoDTO>> GetDisponiblesEnSedeAsync(int idSede, DateTime fechaInicio, DateTime fechaFin, TimeSpan horaInicio, TimeSpan horaFin)
    {
        return await _puestosTrabajoRepository.GetDisponiblesEnSedeAsync(idSede, fechaInicio, fechaFin, horaInicio, horaFin);
    }
    
    }
}
