using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using System.Data;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;


namespace CoWorking.Repositories
{
    public class ZonasTrabajoRepository : IZonasTrabajoRepository
    {
        private readonly CoworkingDBContext _context;


        public ZonasTrabajoRepository(CoworkingDBContext context) // referencia al data.CoworkingDBContext.cs en lugar de cadena de conexión, el EF hará las sentencias sin ponerlas explicitamente
        {
            _context = context;
        }


        public async Task<List<ZonasTrabajo>> GetAllAsync()
        {
            var zonasTrabajo = await _context.ZonasTrabajo

                .Select(u => new ZonasTrabajo
                {
                    IdZonaTrabajo = u.IdZonaTrabajo,
                    Descripcion = u.Descripcion,
                    IdSala = u.IdSala
                })
                .ToListAsync();

            return zonasTrabajo;
        }

        public async Task<ZonasTrabajo?> GetByIdAsync(int id)
        {
            var zonasTrabajo = await _context.ZonasTrabajo
                .Where(u => u.IdZonaTrabajo == id)
                .Select(u => new ZonasTrabajo
                {
                    IdZonaTrabajo = u.IdZonaTrabajo,
                    Descripcion = u.Descripcion,
                    IdSala = u.IdSala
                })
                .FirstOrDefaultAsync();

            return zonasTrabajo;
        }


        public async Task AddAsync(ZonasTrabajoDTO zonaTrabajoDTO)
        {
            var zonasTrabajoEntidad = new ZonasTrabajo
            {
                    IdZonaTrabajo = zonaTrabajoDTO.IdZonaTrabajo,
                    Descripcion = zonaTrabajoDTO.Descripcion,
                    IdSala = zonaTrabajoDTO.IdSala
            };

            await _context.ZonasTrabajo.AddAsync(zonasTrabajoEntidad);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateAsync(ZonasTrabajo zonaTrabajo)
        {
            _context.ZonasTrabajo.Update(zonaTrabajo); // igual que el add pero haciendo un update
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var zonaTrabajo = await GetByIdAsync(id); // primero busca el id del usuario
            if (zonaTrabajo != null)
            {// si existe, pasa a ejecutar

                _context.ZonasTrabajo.Remove(zonaTrabajo); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }
    }
}

