using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using System.Data;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;


namespace CoWorking.Repositories
{
    public class SalasRepository : ISalasRepository
    {
        private readonly CoworkingDBContext _context;


        public SalasRepository(CoworkingDBContext context) // referencia al data.CoworkingDBContext.cs en lugar de cadena de conexión, el EF hará las sentencias sin ponerlas explicitamente
        {
            _context = context;
        }


        public async Task<List<Salas>> GetAllAsync()
        {
            var salas = await _context.Salas

                .Select(u => new Salas
                {
                    IdSala = u.IdSala,
                    Nombre = u.Nombre,
                    URL_Imagen = u.URL_Imagen,
                    Capacidad = u.Capacidad,
                    IdTipoSala = u.IdTipoSala,
                    IdSede = u.IdSede,
                    Bloqueado = u.Bloqueado,
                })
                .ToListAsync();

            return salas;
        }

        public async Task<Salas?> GetByIdAsync(int id)
        {
            var salas = await _context.Salas
                .Where(u => u.IdSala == id)
                .Select(u => new Salas
                {
                    IdSala = u.IdSala,
                    Nombre = u.Nombre,
                    URL_Imagen = u.URL_Imagen,
                    Capacidad = u.Capacidad,
                    IdTipoSala = u.IdTipoSala,
                    IdSede = u.IdSede,
                    Bloqueado = u.Bloqueado,
                })
                .FirstOrDefaultAsync();

            return salas;
        }


        public async Task AddAsync(SalasDTO SalaDTO)
        {
            var salasEntidad = new Salas
            {
                    IdSala = SalaDTO.IdSala,
                    Nombre = SalaDTO.Nombre,
                    URL_Imagen = SalaDTO.URL_Imagen,
                    Capacidad = SalaDTO.Capacidad,
                    IdTipoSala = SalaDTO.IdTipoSala,
                    IdSede = SalaDTO.IdSede,
                    Bloqueado = SalaDTO.Bloqueado,
            };

            await _context.Salas.AddAsync(salasEntidad);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateAsync(Salas sala)
        {
            _context.Salas.Update(sala); // igual que el add pero haciendo un update
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sala = await GetByIdAsync(id); // primero busca el id del usuario
            if (sala != null)
            {// si existe, pasa a ejecutar

                _context.Salas.Remove(sala); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }
    }
}




/*
namespace CoWorking.Repositories
{
    public class SalasRepository : ISalasRepository
    {
        private readonly string _connectionString;

        public SalasRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

public async Task<List<SalasDetallesDTO>> GetAllAsync()
{
    var salas = new List<SalasDetallesDTO>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        
        string query = "SELECT IdSala FROM Salas;";

        using (var command = new SqlCommand(query, connection))
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var sala = new SalasDetallesDTO
                    {
                        IdSala = reader.GetInt32(0),
                        Zona = new List<ZonasTrabajo>(),
                        Puestos = new List<PuestosTrabajo>()
                    };

                    // Zonas de trabajo para esta sala
                    string queryZonasTrabajo = "SELECT IdZonaTrabajo, Descripcion FROM ZonasTrabajo WHERE IdSala = @idSala";
                    using (var commandZonaTrabajo = new SqlCommand(queryZonasTrabajo, connection))
                    {
                        commandZonaTrabajo.Parameters.AddWithValue("@idSala", sala.IdSala);
                        using (var readerZonaTrabajo = await commandZonaTrabajo.ExecuteReaderAsync())
                        {
                            while (await readerZonaTrabajo.ReadAsync())
                            {
                                sala.Zona.Add(new ZonasTrabajo
                                {
                                    IdZonaTrabajo = readerZonaTrabajo.GetInt32(0),
                                    Descripcion = readerZonaTrabajo.GetString(1)
                                });
                            }
                        }
                    }

                    // Puestos de trabajo para esta sala
                    string queryPuestosTrabajo = "SELECT IdPuestoTrabajo, NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, Bloqueado FROM PuestosTrabajo";
                    
                    using (var commandPuestoTrabajo = new SqlCommand(queryPuestosTrabajo, connection))
                    {
                        commandPuestoTrabajo.Parameters.AddWithValue("@idSala", sala.IdSala);
                        using (var readerPuestosTrabajos = await commandPuestoTrabajo.ExecuteReaderAsync())
                        {
                            while (await readerPuestosTrabajos.ReadAsync())
                            {
                                var puesto = new PuestosTrabajo
                                {
                                IdPuestoTrabajo = readerPuestosTrabajos.GetInt32(0),
                                NumeroAsiento = readerPuestosTrabajos.GetInt32(1),
                                CodigoMesa = readerPuestosTrabajos.GetInt32(2),
                                URL_Imagen = readerPuestosTrabajos.GetString(3),
                                Disponible = readerPuestosTrabajos.GetBoolean(4),
                                Bloqueado = readerPuestosTrabajos.GetBoolean(5),
                                };
                                sala.Puestos.Add(puesto);
                            }
                        }
                    }


                    salas.Add(sala);
                }
            }
        }
    }

    return salas;
}
public async Task<SalasDetallesDTO> GetByIdAsync(int id)
{
    SalasDetallesDTO sala = null;

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        string query = @"
        SELECT IdSala FROM Salas WHERE IdSala = @IdSala";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@IdSala", id);
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    sala = new SalasDetallesDTO
                    {
                        IdSala = reader.GetInt32(0),
                        Zona = new List<ZonasTrabajo>(),
                        Puestos = new List<PuestosTrabajo>()
                    };
                }
            }
        }

        if (sala != null)
        {
            // Zonas de trabajo para esta sala
            string queryZonasTrabajo = "SELECT IdZonaTrabajo, Descripcion FROM ZonasTrabajo WHERE IdSala = @idSala";
            using (var commandZonaTrabajo = new SqlCommand(queryZonasTrabajo, connection))
            {
                commandZonaTrabajo.Parameters.AddWithValue("@idSala", sala.IdSala);
                using (var readerZonaTrabajo = await commandZonaTrabajo.ExecuteReaderAsync())
                {
                    while (await readerZonaTrabajo.ReadAsync())
                    {
                        var zonaTrabajo = new ZonasTrabajo
                        {
                            IdZonaTrabajo = readerZonaTrabajo.GetInt32(0),
                            Descripcion = readerZonaTrabajo.GetString(1)
                        };
                        sala.Zona.Add(zonaTrabajo);
                    }
                }
            }

            // Puestos de trabajo para esta sala
            string queryPuestosTrabajo = "SELECT IdPuestoTrabajo, NumeroAsiento, CodigoMesa, URL_Imagen, Disponible, Bloqueado FROM PuestosTrabajo WHERE IdSala = @idSala";
            using (var commandPuestoTrabajo = new SqlCommand(queryPuestosTrabajo, connection))
            {
                commandPuestoTrabajo.Parameters.AddWithValue("@idSala", sala.IdSala);
                using (var readerPuestosTrabajos = await commandPuestoTrabajo.ExecuteReaderAsync())
                {
                    while (await readerPuestosTrabajos.ReadAsync())
                    {
                        var puesto = new PuestosTrabajo
                        {
                            IdPuestoTrabajo = readerPuestosTrabajos.GetInt32(0),
                            NumeroAsiento = readerPuestosTrabajos.GetInt32(1),
                            CodigoMesa = readerPuestosTrabajos.GetInt32(2),
                            URL_Imagen = readerPuestosTrabajos.GetString(3),
                            Disponible = readerPuestosTrabajos.GetBoolean(4),
                            Bloqueado = readerPuestosTrabajos.GetBoolean(5),
                        };
                        sala.Puestos.Add(puesto);
                    }
                }
            }

        }
    }

    return sala; // Retorna null si no encuentra la sala
}

public async Task<List<SalasDTO>> GetByIdSedeAsync(int id)
{
    List<SalasDTO> salas = new List<SalasDTO>();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        string query = @"
        SELECT IdSala, Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado 
        FROM Salas 
        WHERE IdSede = @IdSede";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@IdSede", id);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var salaDto = new SalasDTO
                    {
                        IdSala = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        URL_Imagen = reader.GetString(2),
                        Capacidad = reader.GetInt32(3),
                        IdTipoSala = reader.GetInt32(4),
                        IdSede = reader.GetInt32(5),
                        Bloqueado = reader.GetBoolean(6),
                    };

                    salas.Add(salaDto);
                }
            }
        }

      

    return salas;
}
}



public async Task AddAsync(Salas sala)
{
    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        int numeroMesas = 0;
        int capacidadAsientos = 0;

        string queryTipoSala = @"
        SELECT NumeroMesas, CapacidadAsientos FROM Salas WHERE IdTipoSala = @IdTipoSala";

        using (var command = new SqlCommand(queryTipoSala, connection))
        {
            command.Parameters.AddWithValue("@IdTipoSala", sala.IdTipoSala);
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    numeroMesas = reader.GetInt32(0);
                    capacidadAsientos = reader.GetInt32(1);
                }
                else
                {
                    throw new Exception($"No se encontró un TipoSala con IdTipoSala = {sala.IdTipoSala}");
                }
            }
        }

        int capacidadTotal = numeroMesas * capacidadAsientos;

        string insertSala = @"
        INSERT INTO Salas (Nombre, URL_Imagen, Capacidad, IdTipoSala, IdSede, Bloqueado) 
        VALUES (@Nombre, @URL_Imagen, @Capacidad, @IdTipoSala, @IdSede, @Bloqueado);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        int idSala;

        using (var command = new SqlCommand(insertSala, connection))
        {
            command.Parameters.AddWithValue("@Nombre", sala.Nombre);
            command.Parameters.AddWithValue("@URL_Imagen", sala.URL_Imagen);
            command.Parameters.AddWithValue("@Capacidad", capacidadTotal);
            command.Parameters.AddWithValue("@IdTipoSala", sala.IdTipoSala);
            command.Parameters.AddWithValue("@IdSede", sala.IdSede);
            command.Parameters.AddWithValue("@Bloqueado", false);

            idSala = (int)await command.ExecuteScalarAsync();
        }

        string insertZonaTrabajo = @"
        INSERT INTO ZonasTrabajo (Descripcion, IdSala) 
        VALUES (@Descripcion, @IdSala);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        int idZonaTrabajo;
        using (var command = new SqlCommand(insertZonaTrabajo, connection))
        {
            command.Parameters.AddWithValue("@Descripcion", "");
            command.Parameters.AddWithValue("@IdSala", idSala);

            idZonaTrabajo = (int)await command.ExecuteScalarAsync();
        }

                //  se crean las sillas en base a la capacidad total
        for (int i = 0; i < capacidadTotal; i++)
        {
                    // cada silla tendrá asociada una mesa a la que se asocie
            int codigoMesa = (i / capacidadAsientos) + 1; // al ser una operacion de enteros, no hay decimales, por tanto 3/4 = 0, 5/4 = 1 y asi con todos, pudiendo asi autonincrementar los valores
            int numeroAsiento = i + 1; // desde 1 hasta el final, id secundario para facilitar el fetch

            string insertPuestoTrabajo = @"
            INSERT INTO PuestosTrabajo (NumeroAsiento, URL_Imagen, CodigoMesa, IdZonaTrabajo, IdSala, Bloqueado) 
            VALUES (@NumeroAsiento, @URL_Imagen, @CodigoMesa, @IdZonaTrabajo, @IdSala, @Bloqueado);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int idPuestoTrabajo;
            using (var command = new SqlCommand(insertPuestoTrabajo, connection))
            {
                command.Parameters.AddWithValue("@NumeroAsiento", numeroAsiento);
                command.Parameters.AddWithValue("@URL_Imagen", "");
                command.Parameters.AddWithValue("@CodigoMesa", codigoMesa);
                command.Parameters.AddWithValue("@IdZonaTrabajo", idZonaTrabajo);
                command.Parameters.AddWithValue("@IdSala", idSala);
                command.Parameters.AddWithValue("@Bloqueado", false);

                idPuestoTrabajo = (int)await command.ExecuteScalarAsync();
            }

            for (int diasMes = 1; diasMes <= 31; diasMes++)
            {
                string insertDisponibilidad = @"
                INSERT INTO Disponibilidades (Fecha, Estado, IdTramoHorario, IdPuestoTrabajo) 
                VALUES (@Fecha, @Estado, @IdTramoHorario, @IdPuestoTrabajo)";

                using (var command = new SqlCommand(insertDisponibilidad, connection))
                {
                    command.Parameters.AddWithValue("@Fecha", diasMes);
                    command.Parameters.AddWithValue("@Estado", true);
                    command.Parameters.AddWithValue("@IdTramoHorario", 1);
                    command.Parameters.AddWithValue("@IdPuestoTrabajo", idPuestoTrabajo);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        sala.Capacidad = capacidadTotal;
    }
}
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // get para el id del tipo de sala asociado a la sala
                int? idTipoSala = null; // ? es un nullable, permite inicializar la variable entera con nulo
                string getTipoSalaQuery = "SELECT IdTipoSala FROM Salas WHERE IdSala = @IdSala";
                using (var command = new SqlCommand(getTipoSalaQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdSala", id);
                    var respuestaIdTipoSala = await command.ExecuteScalarAsync();
                    if (respuestaIdTipoSala != null) // si encuentra un valor
                    {
                        idTipoSala = (int)respuestaIdTipoSala; // lo asigna a la variable
                    }
                }

                // borrar los asientos
                string deletePuestosTrabajo = "DELETE FROM PuestosTrabajo WHERE IdSala = @IdSala";
                using (var command = new SqlCommand(deletePuestosTrabajo, connection))
                {
                    command.Parameters.AddWithValue("@IdSala", id);
                    await command.ExecuteNonQueryAsync();
                }

                // borrar la zona
                string deleteZonasTrabajo = "DELETE FROM ZonasTrabajo WHERE IdSala = @IdSala";
                using (var command = new SqlCommand(deleteZonasTrabajo, connection))
                {
                    command.Parameters.AddWithValue("@IdSala", id);
                    await command.ExecuteNonQueryAsync();
                }

                // borrar la sala (en este orden para evitar conflictos de clave foranea)
                string deleteSala = "DELETE FROM Salas WHERE IdSala = @IdSala";
                using (var command = new SqlCommand(deleteSala, connection))
                {
                    command.Parameters.AddWithValue("@IdSala", id);
                    await command.ExecuteNonQueryAsync();
                }

                // vborrar el tipo de sala, pero solo si no se usa en otra sala comprobandolo para evitar posibles errores, ya que al principio las posibles salas serán predefinidas
                if (idTipoSala.HasValue)
                {
                    string comprobarUsoSalas = "SELECT COUNT(*) FROM Salas WHERE IdTipoSala = @IdTipoSala";
                    using (var command = new SqlCommand(comprobarUsoSalas, connection))
                    {
                        command.Parameters.AddWithValue("@IdTipoSala", idTipoSala.Value);
                        int conteoSalas = (int)await command.ExecuteScalarAsync();
                        if (conteoSalas == 0) // si no se encuentran resultados, significa que no se esta usando por otros datos y por tanto se puede eliminar sin problemas
                        {
                            string deleteTipoSala = "DELETE FROM Salas WHERE IdTipoSala = @IdTipoSala";
                            using (var ejecutarBorradoTipoSala = new SqlCommand(deleteTipoSala, connection))
                            {
                                ejecutarBorradoTipoSala.Parameters.AddWithValue("@IdTipoSala", idTipoSala.Value);
                                await ejecutarBorradoTipoSala.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
        }
    }
}
*/