using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;
using Dtos;


namespace CoWorking.Repositories
{
    public class PuestosTrabajoRepository : IPuestosTrabajoRepository
    {
        private readonly CoworkingDBContext _context;


        public PuestosTrabajoRepository(CoworkingDBContext context) // referencia al data.CoworkingDBContext.cs en lugar de cadena de conexión, el EF hará las sentencias sin ponerlas explicitamente
        {
            _context = context;
        }


        public async Task<List<PuestosTrabajoDTO>> GetAllAsync()
        {
            return await _context.PuestosTrabajo
                .Select(p => new PuestosTrabajoDTO
                {
                    IdPuestoTrabajo = p.IdPuestoTrabajo,
                    NumeroAsiento = p.NumeroAsiento,
                    CodigoMesa = p.CodigoMesa,
                    URL_Imagen = p.URL_Imagen,
                    Disponible = p.Disponible,
                    Bloqueado = p.Bloqueado,
                    IdZonaTrabajo = p.IdZonaTrabajo,
                    IdSala = p.IdSala
                })
                .ToListAsync();
        }


        public async Task<PuestosTrabajoDTO> GetByIdAsync(int id)
        {
            return await _context.PuestosTrabajo
                .Where(p => p.IdPuestoTrabajo == id)
                .Select(p => new PuestosTrabajoDTO
                {
                    IdPuestoTrabajo = p.IdPuestoTrabajo,
                    NumeroAsiento = p.NumeroAsiento,
                    CodigoMesa = p.CodigoMesa,
                    URL_Imagen = p.URL_Imagen,
                    Disponible = p.Disponible,
                    Bloqueado = p.Bloqueado,
                    IdZonaTrabajo = p.IdZonaTrabajo,
                    IdSala = p.IdSala
                })
                .FirstOrDefaultAsync();
        }


        public async Task AddAsync(PuestosTrabajoDTO puestoTrabajo)
        {
            var entity = new PuestosTrabajo
            {
                NumeroAsiento = puestoTrabajo.NumeroAsiento,
                CodigoMesa = puestoTrabajo.CodigoMesa,
                URL_Imagen = puestoTrabajo.URL_Imagen,
                Disponible = puestoTrabajo.Disponible,
                Bloqueado = puestoTrabajo.Bloqueado,
                IdZonaTrabajo = puestoTrabajo.IdZonaTrabajo,
                IdSala = puestoTrabajo.IdSala
            };

            await _context.PuestosTrabajo.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PuestosTrabajoDTO puestoTrabajo)
        {
            var puesto = await _context.PuestosTrabajo.FindAsync(puestoTrabajo.IdPuestoTrabajo);

            if (puesto == null)
                throw new Exception($"No se encontró el puesto con ID {puestoTrabajo.IdPuestoTrabajo}.");

            puesto.NumeroAsiento = puestoTrabajo.NumeroAsiento;
            puesto.CodigoMesa = puestoTrabajo.CodigoMesa;
            puesto.URL_Imagen = puestoTrabajo.URL_Imagen;
            puesto.Disponible = puestoTrabajo.Disponible;
            puesto.Bloqueado = puestoTrabajo.Bloqueado;
            puesto.IdZonaTrabajo = puestoTrabajo.IdZonaTrabajo;
            puesto.IdSala = puestoTrabajo.IdSala;

            _context.PuestosTrabajo.Update(puesto);
            await _context.SaveChangesAsync();
        }

public async Task DeleteAsync(int id)
{
    var puesto = await _context.PuestosTrabajo.FindAsync(id);

    if (puesto == null)
        throw new Exception("No se encontró ese puesto de trabajo");

    // Eliminar las disponibilidades asociadas con este puesto de trabajo
    var disponibilidades = _context.Disponibilidades.Where(d => d.IdPuestoTrabajo == id);
    _context.Disponibilidades.RemoveRange(disponibilidades); // elimina todas las disponibilidades asociadas con FK

    var lineas = _context.Lineas.Where(l => l.IdPuestoTrabajo == id);
    _context.Lineas.RemoveRange(lineas); // elimina todas las lineas asociadas con FK

    _context.PuestosTrabajo.Remove(puesto); // elimina el puesto de trabajo
    await _context.SaveChangesAsync();
}


    }
}