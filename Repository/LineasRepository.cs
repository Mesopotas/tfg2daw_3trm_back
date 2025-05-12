using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;

namespace CoWorking.Repositories
{
    public class LineasRepository : ILineasRepository
    {
        private readonly CoworkingDBContext _context;

        public LineasRepository(CoworkingDBContext context)
        {
            _context = context;
        }

        public async Task<List<LineasDTO>> GetAllAsync()
        {
            return await _context.Lineas
                .Select(linea => new LineasDTO
                {
                    IdLinea = linea.IdLinea,
                    IdReserva = linea.IdReserva,
                    IdPuestoTrabajo = linea.IdPuestoTrabajo,
                    Precio = linea.Precio
                })
                .ToListAsync();
        }

        public async Task<LineasDTO> GetByIdAsync(int id)
        {
            return await _context.Lineas
                                 .Where(l => l.IdLinea == id)
                                 .Select(l => new LineasDTO
                                 {
                                     IdLinea = l.IdLinea,
                                     IdReserva = l.IdReserva,
                                     IdPuestoTrabajo = l.IdPuestoTrabajo,
                                     Precio = l.Precio
                                 })
                                 .FirstOrDefaultAsync();
        }

        public async Task AddAsync(LineasDTO linea)
        {
            // obtener el precio desde la tabla Reservas
            var reserva = await _context.Reservas
                                        .Where(r => r.IdReserva == linea.IdReserva)
                                        .Select(r => r.PrecioTotal)
                                        .FirstOrDefaultAsync();

            // si el precio existe, crear la linea
            if (reserva != null)
            {
                var lineaobjeto = new Lineas
                {
                    IdReserva = linea.IdReserva,
                    IdPuestoTrabajo = linea.IdPuestoTrabajo,
                    Precio = reserva
                };

                _context.Lineas.Add(lineaobjeto);
                await _context.SaveChangesAsync();
            }
        }




        public async Task DeleteAsync(int id)
        {
            var linea = await _context.Lineas.FindAsync(id);
            if (linea != null)
            {
                _context.Lineas.Remove(linea);
                await _context.SaveChangesAsync();
            }
        }

    }
}
