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

/*
        public async Task<ReservasDTO> GetByIdAsync(int id)
        {
            ReservasDTO reserva = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // ELIMINADO DETALLES RESERVA , REESTRUCTURAR JOIN / 

                string query = @"
            SELECT  
                reserva.IdReserva, reserva.Fecha, reserva.Descripcion, reserva.PrecioTotal,
                usuario.IdUsuario, usuario.Nombre , usuario.Email,
               puesto.IdPuestoTrabajo, puesto.CodigoMesa, 
                puesto.URL_Imagen,
            FROM Reservas reserva
            INNER JOIN Usuarios usuario ON reserva.IdUsuario = usuario.IdUsuario
            INNER JOIN Lineas linea ON reserva.IdReserva = linea.IdReserva
            WHERE reserva.IdReserva = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            reserva = new ReservasDTO
                            {
                                IdReserva = reader.GetInt32(0),
                                Fecha = reader.GetDateTime(1),
                                ReservaDescripcion = reader.GetString(2),
                                PrecioTotal = (double)reader.GetDecimal(3),
                                UsuarioId = reader.GetInt32(4),
                                UsuarioNombre = reader.GetString(5),
                                UsuarioEmail = reader.GetString(6),
                            };


                        }
                    }
                }
            }
            return reserva;
        }

        public async Task CreateReservaAsync(Reservas reserva)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Obtener el precio del asiento con inner joins
                string queryPrecio = @"
            SELECT TPT.Precio
            FROM PuestosTrabajo PT
            INNER JOIN ZonasTrabajo ZT ON PT.IdZonaTrabajo = ZT.IdZonaTrabajo
            INNER JOIN Salas S ON ZT.IdSala = S.IdSala
            INNER JOIN TiposSalas TS ON S.IdTipoSala = TS.IdTipoSala
            INNER JOIN TiposPuestosTrabajo TPT ON TS.IdTipoPuestoTrabajo = TPT.IdTipoPuestoTrabajo
            WHERE PT.IdPuestoTrabajo = @IdPuestoTrabajo;";

                decimal precio = 0;

                using (var command = new SqlCommand(queryPrecio, connection))
                {
                    command.Parameters.AddWithValue("@IdPuestoTrabajo", reserva.IdPuestoTrabajo);
                    var result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        precio = Convert.ToDecimal(result);
                        Console.WriteLine($"Precio encontrado para puesto {reserva.IdPuestoTrabajo}: {precio}");
                    }
                    else
                    {
                        throw new Exception($"No se encontró el precio para el puesto de trabajo ID: {reserva.IdPuestoTrabajo}");
                    }
                }

                // Insertar la nueva reserva
                string queryInsert = @"
            INSERT INTO Reservas (IdUsuario, Fecha, Descripcion, PrecioTotal)
            VALUES (@IdUsuario, @Fecha, @Descripcion, @PrecioTotal);
            SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(queryInsert, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", reserva.IdUsuario);
                    command.Parameters.AddWithValue("@Fecha", reserva.Fecha);
                    command.Parameters.AddWithValue("@Descripcion", reserva.Descripcion);
                    command.Parameters.AddWithValue("@PrecioTotal", precio);

                    // Get the ID of the newly created reservation
                    var newReservaId = await command.ExecuteScalarAsync();
                    if (newReservaId != null && newReservaId != DBNull.Value)
                    {
                        reserva.IdReserva = Convert.ToInt32(newReservaId);
                    }
                }
            }
        }

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

