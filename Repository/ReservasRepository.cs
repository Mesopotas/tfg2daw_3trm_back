using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public class ReservasRepository : IReservasRepository
    {
        private readonly string _connectionString;

        public ReservasRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<ReservasDTO>> GetAllAsync()
        {
            var reservas = new List<ReservasDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                 SELECT  
                reserva.IdReserva, reserva.Fecha, reserva.Descripcion, reserva.PrecioTotal,
                usuario.IdUsuario, usuario.Nombre , usuario.Email,
                detallesr.IdDetalleReserva, puesto.IdPuestoTrabajo, puesto.CodigoMesa, 
                puesto.URL_Imagen, detallesr.Descripcion
                FROM Reservas reserva
                INNER JOIN Usuarios usuario ON reserva.IdUsuario = usuario.IdUsuario
                INNER JOIN Lineas linea ON reserva.IdReserva = linea.IdReserva
                INNER JOIN DetallesReservas detallesr ON linea.IdDetalleReserva = detallesr.IdDetalleReserva
                INNER JOIN PuestosTrabajo puesto ON detallesr.IdPuestoTrabajo = puesto.IdPuestoTrabajo;";


                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var reserva = new ReservasDTO
                        {
                            IdReserva = reader.GetInt32(0),
                            Fecha = reader.GetDateTime(1),
                            ReservaDescripcion = reader.GetString(2),
                            PrecioTotal = (double)reader.GetDecimal(3),
                            UsuarioId = reader.GetInt32(4),
                            UsuarioNombre = reader.GetString(5),
                            UsuarioEmail = reader.GetString(6),
                            DetallesReservas = new List<DetalleReservaDTO>()
                        };

                        if (!reader.IsDBNull(7))   // busca el detalle de la reserva si existe
                        {
                            reserva.DetallesReservas.Add(new DetalleReservaDTO
                            {
                                IdDetalleReserva = reader.GetInt32(7),
                                IdPuestoTrabajo = reader.GetInt32(8),
                                CodigoPuesto = reader.GetString(9),
                                ImagenPuesto = reader.GetString(10),
                                Descripcion = reader.GetString(11)
                            });
                        }

                        reservas.Add(reserva);
                    }
                }
            }
            return reservas;
        }

        public async Task<ReservasDTO> GetByIdAsync(int id)
        {
            ReservasDTO reserva = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
            SELECT  
                reserva.IdReserva, reserva.Fecha, reserva.Descripcion, reserva.PrecioTotal,
                usuario.IdUsuario, usuario.Nombre , usuario.Email,
                detallesr.IdDetalleReserva, puesto.IdPuestoTrabajo, puesto.CodigoMesa, 
                puesto.URL_Imagen, detallesr.Descripcion
            FROM Reservas reserva
            INNER JOIN Usuarios usuario ON reserva.IdUsuario = usuario.IdUsuario
            INNER JOIN Lineas linea ON reserva.IdReserva = linea.IdReserva
            INNER JOIN DetallesReservas detallesr ON linea.IdDetalleReserva = detallesr.IdDetalleReserva
            INNER JOIN PuestosTrabajo puesto ON detallesr.IdPuestoTrabajo = puesto.IdPuestoTrabajo
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
                                DetallesReservas = new List<DetalleReservaDTO>()
                            };


                            if (!reader.IsDBNull(7))
                            {
                                reserva.DetallesReservas.Add(new DetalleReservaDTO
                                {
                                    IdDetalleReserva = reader.GetInt32(7),
                                    IdPuestoTrabajo = reader.GetInt32(8),
                                    CodigoPuesto = reader.GetString(9),
                                    ImagenPuesto = reader.GetString(10),
                                    Descripcion = reader.GetString(11)
                                });
                            }
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
        public async Task<ReservasClienteInfoDTO> GetDetallesPedidoAsync(int idReserva, int idDetalleReserva)
        {
            ReservasClienteInfoDTO reserva = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"SELECT r.PrecioTotal, u.Nombre AS UsuarioNombre, u.Apellidos AS UsuarioApellidos, 
       u.Email AS UsuarioEmail, pt.NumeroAsiento, s.Nombre AS NombreSala, 
       ts.Nombre AS TipoSala, tpt.Precio AS PrecioPuesto
FROM Reservas r, Usuarios u, DetallesReservas dr, PuestosTrabajo pt, Salas s, 
     TiposSalas ts, TiposPuestosTrabajo tpt
WHERE r.IdUsuario = u.IdUsuario
  AND dr.IdPuestoTrabajo = pt.IdPuestoTrabajo
  AND pt.IdSala = s.IdSala
  AND s.IdTipoSala = ts.IdTipoSala 
  AND ts.IdTipoPuestoTrabajo = tpt.IdTipoPuestoTrabajo 
  AND r.IdReserva = @IdReserva 
  AND dr.IdDetalleReserva = @IdDetalleReserva;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdReserva", idReserva);
                    command.Parameters.AddWithValue("@IdDetalleReserva", idDetalleReserva);

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
                                    Detalles = new List<DetallesReservaClienteDTO>()
                                };
                            }

                            // informacion del asiento escogido
                            reserva.Detalles.Add(new DetallesReservaClienteDTO
                            {
                                NumeroAsiento = reader.GetInt32(4),   
                                NombreSala = reader.GetString(5),    
                                TipoSala = reader.GetString(6),    
                                PrecioPuesto = reader.GetDecimal(7) 
                            });
                        }
                    }
                }
            }

            return reserva;
        }

    }
}