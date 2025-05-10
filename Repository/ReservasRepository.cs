using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;


namespace CoWorking.Repositories
{
    public class ReservasRepository : IReservasRepository
    {
        private readonly CoworkingDBContext _context;

        public ReservasRepository(CoworkingDBContext context)
        {
            _context = context;
        }

public async Task<List<ReservasDTO>> GetAllAsync()
{
    var reservas = await _context.Reservas
        .Include(r => r.Usuario)
        .Select(r => new ReservasDTO
        {
            IdReserva = r.IdReserva,
            Fecha = r.Fecha,
            ReservaDescripcion = r.Descripcion,
            PrecioTotal = Convert.ToDouble(r.PrecioTotal), // de decimal a double
            UsuarioId = r.Usuario.IdUsuario,
            UsuarioNombre = r.Usuario.Nombre,
            UsuarioEmail = r.Usuario.Email
        })
        .ToListAsync();

    return reservas;
}

public async Task<ReservasDTO?> GetByIdAsync(int id)
{
    var reserva = await _context.Reservas
        .Include(r => r.Usuario)
        .Where(r => r.IdReserva == id)
        .Select(r => new ReservasDTO
        {
            IdReserva = r.IdReserva,
            Fecha = r.Fecha,
            ReservaDescripcion = r.Descripcion,
            PrecioTotal = Convert.ToDouble(r.PrecioTotal), // de decimal a double
            UsuarioId = r.Usuario.IdUsuario,
            UsuarioNombre = r.Usuario.Nombre,
            UsuarioEmail = r.Usuario.Email
        })
        .FirstOrDefaultAsync();

    return reserva;
}
public async Task<Reservas> CreateReservaAsync(Reservas reserva)
{
    _context.Reservas.Add(reserva);
    await _context.SaveChangesAsync();
    return reserva;
}
/*

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Reservas WHERE IdReserva = @IdReserva";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdReserva", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<ReservasClienteInfoDTO> GetDetallesPedidoAsync(int idReserva)
        {
            ReservasClienteInfoDTO reserva = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                / ELIMINADO DETALLES RESERVA , REESTRUCTURAR JOIN / 

                string query = @"SELECT r.PrecioTotal, u.Nombre AS UsuarioNombre, u.Apellidos AS UsuarioApellidos, 
       u.Email AS UsuarioEmail, pt.NumeroAsiento, s.Nombre AS NombreSala, 
       ts.Nombre AS TipoSala, tpt.Precio AS PrecioPuesto
FROM Reservas r, Usuarios u, PuestosTrabajo pt, Salas s, 
     TiposSalas ts, TiposPuestosTrabajo tpt
WHERE r.IdUsuario = u.IdUsuario
  AND dr.IdPuestoTrabajo = pt.IdPuestoTrabajo
  AND pt.IdSala = s.IdSala
  AND s.IdTipoSala = ts.IdTipoSala 
  AND ts.IdTipoPuestoTrabajo = tpt.IdTipoPuestoTrabajo 
  AND r.IdReserva = @IdReserva ;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdReserva", idReserva);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reserva == null)
                            {       // data del cliente
                                reserva = new ReservasClienteInfoDTO
                                {
                                    PrecioTotal = reader.GetDecimal(0),   
                                    UsuarioNombre = reader.GetString(1),    
                                    UsuarioApellidos = reader.GetString(2), 
                                    UsuarioEmail = reader.GetString(3),     
                                };
                            }

                          
                        }
                    }
                }
            }

            return reserva;
        }
        */
    }
}

