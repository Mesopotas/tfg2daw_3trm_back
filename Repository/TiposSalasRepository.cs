using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using System.Data;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;


namespace CoWorking.Repositories
{
    public class TiposSalasRepository : ITiposSalasRepository
    {
        private readonly CoworkingDBContext _context;


        public TiposSalasRepository(CoworkingDBContext context) // referencia al data.CoworkingDBContext.cs en lugar de cadena de conexión, el EF hará las sentencias sin ponerlas explicitamente
        {
            _context = context;
        }


        public async Task<List<TiposSalas>> GetAllAsync()
        {
            var tiposSalas = await _context.TiposSalas

                .Select(u => new TiposSalas
                {
                    IdTipoSala = u.IdTipoSala,
                    Nombre = u.Nombre,
                    NumeroMesas = u.NumeroMesas,
                    CapacidadAsientos = u.CapacidadAsientos,
                    EsPrivada = u.EsPrivada,
                    Descripcion = u.Descripcion,
                    IdTipoPuestoTrabajo = u.IdTipoPuestoTrabajo,
                })
                .ToListAsync();

            return tiposSalas;
        }

        public async Task<TiposSalas?> GetByIdAsync(int id)
        {
            var tiposSalas = await _context.TiposSalas
                .Where(u => u.IdTipoSala == id)
                .Select(u => new TiposSalas
                {
                    IdTipoSala = u.IdTipoSala,
                    Nombre = u.Nombre,
                    NumeroMesas = u.NumeroMesas,
                    CapacidadAsientos = u.CapacidadAsientos,
                    EsPrivada = u.EsPrivada,
                    Descripcion = u.Descripcion,
                    IdTipoPuestoTrabajo = u.IdTipoPuestoTrabajo,
                })
                .FirstOrDefaultAsync();

            return tiposSalas;
        }


        public async Task AddAsync(TiposSalasDTO tipoSalaDTO)
        {
            var tiposSalasEntidad = new TiposSalas
            {
                IdTipoSala = tipoSalaDTO.IdTipoSala,
                Nombre = tipoSalaDTO.Nombre,
                NumeroMesas = tipoSalaDTO.NumeroMesas,
                CapacidadAsientos = tipoSalaDTO.CapacidadAsientos,
                EsPrivada = tipoSalaDTO.EsPrivada,
                Descripcion = tipoSalaDTO.Descripcion,
                IdTipoPuestoTrabajo = tipoSalaDTO.IdTipoPuestoTrabajo
            };

            await _context.TiposSalas.AddAsync(tiposSalasEntidad);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateAsync(TiposSalas tipoSala)
        {
            _context.TiposSalas.Update(tipoSala); // igual que el add pero haciendo un update
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tipoSala = await GetByIdAsync(id); // primero busca el id del usuario
            if (tipoSala != null)
            {// si existe, pasa a ejecutar

                _context.TiposSalas.Remove(tipoSala); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }

        /*

                public async Task<TiposSalas> GetByIdAsync(int id)
                {
                    TiposSalas tipoSala = null;

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        string query = @"
                SELECT IdTipoSala, Nombre, NumeroMesas, CapacidadAsientos, EsPrivada, Descripcion, IdTipoPuestoTrabajo FROM TiposSalas WHERE IdTipoSala = @Id";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    tipoSala = new TiposSalas
                                    {
                                        IdTipoSala = reader.GetInt32(0),
                                        Nombre = reader.IsDBNull(1) ? null : reader.GetString(1), // si es nulo le asigna 'null' para que no se colapse
                                        NumeroMesas = reader.GetInt32(2),
                                        CapacidadAsientos = reader.GetInt32(3),
                                        EsPrivada = reader.GetBoolean(4),
                                        Descripcion = reader.IsDBNull(5) ? null : reader.GetString(5), // si es nulo le asigna 'null' para que no se colapse
                                        IdTipoPuestoTrabajo = reader.GetInt32(6)
                                    };
                                }
                            }
                        }
                    }

                    return tipoSala;
                }
                public async Task AddAsync(TiposSalas tipoSala)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        tipoSala.IdTipoPuestoTrabajo = tipoSala.EsPrivada ? 2 : 1; // si no es privada le dará el id del tipoPuesto 1, q será silla en sala publica, si es privada le dará el valor 2 q es silla en sala privada

                        string query = @"
                INSERT INTO TiposSalas (Nombre, NumeroMesas, CapacidadAsientos, EsPrivada, Descripcion, IdTipoPuestoTrabajo) 
                VALUES (@Nombre, @NumeroMesas, @CapacidadAsientos, @EsPrivada, @Descripcion, @IdTipoPuestoTrabajo)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Nombre", tipoSala.Nombre);
                            command.Parameters.AddWithValue("@NumeroMesas", tipoSala.NumeroMesas);
                            command.Parameters.AddWithValue("@CapacidadAsientos", tipoSala.CapacidadAsientos);
                            command.Parameters.AddWithValue("@EsPrivada", tipoSala.EsPrivada);
                            command.Parameters.AddWithValue("@Descripcion", tipoSala.Descripcion ?? (object)DBNull.Value); // Permitir NULL en Descripcion
                            command.Parameters.AddWithValue("@IdTipoPuestoTrabajo", tipoSala.IdTipoPuestoTrabajo);

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }

                public async Task UpdateAsync(TiposSalas tipoSala)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                    // La columna FechaRegistro no está incluida ya que no debe ser modificada
                string query = "UPDATE TiposSalas SET Nombre = @Nombre, NumeroMesas = @NumeroMesas, CapacidadAsientos = @CapacidadAsientos, EsPrivada = @EsPrivada, Descripcion = @Descripcion, IdTipoPuestoTrabajo = @IdTipoPuestoTrabajo WHERE IdTipoSala = @IdTipoSala";
                    // si el idRol asignado no existe dará error (Microsoft.Data.SqlClient.SqlException (0x80131904): The INSERT statement conflicted with the FOREIGN KEY constraint "FK__TiposSalas__IdRol__276EDEB3". The conflict occurred in database "CoworkingDB", table "dbo.Roles", column 'IdRol'.)
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTipoSala", tipoSala.IdTipoSala);
                    command.Parameters.AddWithValue("@Nombre", tipoSala.Nombre);
                    command.Parameters.AddWithValue("@NumeroMesas", tipoSala.NumeroMesas);
                    command.Parameters.AddWithValue("@CapacidadAsientos", tipoSala.CapacidadAsientos);
                    command.Parameters.AddWithValue("@EsPrivada", tipoSala.EsPrivada);
                    command.Parameters.AddWithValue("@Descripcion", tipoSala.Descripcion);
                    command.Parameters.AddWithValue("@IdTipoPuestoTrabajo", tipoSala.IdTipoPuestoTrabajo);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
                public async Task DeleteAsync(int id)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        // si nuestro tipo sala esta vinculada a alguna sala, no se podrá eliminar y lanzará una excepción, ya que sino daría error o podria causar fallos graves en el funcionamiento de la salas
                        string revisarVinculos = "SELECT COUNT(*) FROM Salas WHERE IdTipoSala = @IdTipoSala"; 
                        using (var checkCommand = new SqlCommand(revisarVinculos, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@IdTipoSala", id);
                            int numeroVinculos = (int)await checkCommand.ExecuteScalarAsync();

                            if (numeroVinculos > 0)
                            {
                                throw new InvalidOperationException($"No se puede eliminar el TipoSala con Id {id} porque tiene algun vinculo a alguna sala");
                            }
                        }

                        // si no hay vinculos, se procederá a borrar la sala
                        string deleteQuery = "DELETE FROM TiposSalas WHERE IdTipoSala = @IdTipoSala";
                        using (var command = new SqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@IdTipoSala", id);
                            int rowsAffected = await command.ExecuteNonQueryAsync();
                        }
                    }
                }*/
    }
}
