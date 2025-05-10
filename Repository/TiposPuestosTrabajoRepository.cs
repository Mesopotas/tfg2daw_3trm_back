using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;

namespace CoWorking.Repositories
{
    public class TiposPuestosTrabajoRepository : ITiposPuestosTrabajoRepository
    {
        private readonly CoworkingDBContext _context;

        public TiposPuestosTrabajoRepository(CoworkingDBContext context)
        {
            _context = context;
        }

        public async Task<List<TiposPuestosTrabajo>> GetAllAsync()
        {
            var resultado = await _context.TiposPuestosTrabajo
                .Select(u => new
                {
                    u.IdTipoPuestoTrabajo,
                    u.Nombre,
                    u.Imagen_URL,
                    u.Descripcion,
                    Precio = Convert.ToDouble(u.Precio)
                })
                .ToListAsync();

            var tiposPuestosTrabajo = resultado
                .Select(r => new TiposPuestosTrabajo
                {
                    IdTipoPuestoTrabajo = r.IdTipoPuestoTrabajo,
                    Nombre = r.Nombre,
                    Imagen_URL = r.Imagen_URL,
                    Descripcion = r.Descripcion,
                    Precio = r.Precio
                })
                .ToList();

            return tiposPuestosTrabajo;
        }

        public async Task<TiposPuestosTrabajo?> GetByIdAsync(int id)
        {
            var resultado = await _context.TiposPuestosTrabajo
                .Where(u => u.IdTipoPuestoTrabajo == id)
                .Select(u => new
                {
                    u.IdTipoPuestoTrabajo,
                    u.Nombre,
                    u.Imagen_URL,
                    u.Descripcion,
                    Precio = Convert.ToDouble(u.Precio)
                })
                .FirstOrDefaultAsync();

            if (resultado == null) return null;

            return new TiposPuestosTrabajo
            {
                IdTipoPuestoTrabajo = resultado.IdTipoPuestoTrabajo,
                Nombre = resultado.Nombre,
                Imagen_URL = resultado.Imagen_URL,
                Descripcion = resultado.Descripcion,
                Precio = resultado.Precio
            };
        }

        public async Task AddAsync(TiposPuestosTrabajo tipoPuestoTrabajo)
        {
            await _context.TiposPuestosTrabajo.AddAsync(tipoPuestoTrabajo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TiposPuestosTrabajo tipoPuestoTrabajo)
        {
            _context.TiposPuestosTrabajo.Update(tipoPuestoTrabajo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tipoPuestoTrabajo = await GetByIdAsync(id);
            if (tipoPuestoTrabajo != null)
            {
                _context.TiposPuestosTrabajo.Remove(tipoPuestoTrabajo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
