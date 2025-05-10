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


        public TiposPuestosTrabajoRepository(CoworkingDBContext context) // referencia al data.CoworkingDBContext.cs en lugar de cadena de conexión, el EF hará las sentencias sin ponerlas explicitamente
        {
            _context = context;
        }


        public async Task<List<TiposPuestosTrabajo>> GetAllAsync()
        {
            var tiposPuestosTrabajo = await _context.TiposPuestosTrabajo

                .Select(u => new TiposPuestosTrabajo
                {
                    IdTipoPuestoTrabajo = u.IdTipoPuestoTrabajo,
                    Nombre = u.Nombre,
                    Imagen_URL = u.Imagen_URL,
                    Descripcion = u.Descripcion,
                    Precio = Convert.ToDouble(u.Precio), // le llega un decimal pero la api maneja un double
                })
                .ToListAsync();

            return tiposPuestosTrabajo;
        }


        public async Task<TiposPuestosTrabajo?> GetByIdAsync(int id)
        {
            var tiposPuestosTrabajo = await _context.TiposPuestosTrabajo
                .Where(u => u.IdTipoPuestoTrabajo == id)
                .Select(u => new TiposPuestosTrabajo
                {
                    IdTipoPuestoTrabajo = u.IdTipoPuestoTrabajo,
                    Nombre = u.Nombre,
                    Imagen_URL = u.Imagen_URL,
                    Descripcion = u.Descripcion,
                    Precio = Convert.ToDouble(u.Precio), // le llega un decimal pero la api maneja un double
                })
                .FirstOrDefaultAsync();

            return tiposPuestosTrabajo;
        }


        public async Task AddAsync(TiposPuestosTrabajo tipoPuestoTrabajo)
        {

            await _context.TiposPuestosTrabajo.AddAsync(tipoPuestoTrabajo); // AddAsync es metodo propio de EF, no hace el insert en si, solo lo prepara
            await _context.SaveChangesAsync(); // otro metodo de EF, esto si hace el insert con los datos del add, ambos son imprescindibles para el insert
        }


        public async Task UpdateAsync(TiposPuestosTrabajo tipoPuestoTrabajo)
        {
            _context.TiposPuestosTrabajo.Update(tipoPuestoTrabajo); // igual que el add pero haciendo un update
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tipoPuestoTrabajo = await GetByIdAsync(id); // primero busca el id del usuario
            if (tipoPuestoTrabajo != null)
            {// si existe, pasa a ejecutar

                _context.TiposPuestosTrabajo.Remove(tipoPuestoTrabajo); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }
    }
}