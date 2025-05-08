using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using System.Data;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;

namespace CoWorking.Repositories
{
    public class DisponibilidadesRepository : IDisponibilidadesRepository
    {
        private readonly CoworkingDBContext _context;

        public DisponibilidadesRepository(CoworkingDBContext context)
        {
            _context = context;
        }

    public async Task<List<DisponibilidadDTO>> GetAllAsync()
{
    return await _context.Disponibilidades
        .Select(d => new DisponibilidadDTO
        {
            IdDisponibilidad = d.IdDisponibilidad,
            Fecha = d.Fecha,
            Estado = d.Estado,
            IdTramoHorario = d.IdTramoHorario
        })
        .ToListAsync();
}



        public async Task<DisponibilidadDTO> GetByIdAsync(int id)
        {
            return await _context.Disponibilidades
                .Where(d => d.IdDisponibilidad == id)
                .Select(d => new DisponibilidadDTO
                {
                    IdDisponibilidad = d.IdDisponibilidad,
                    Fecha = d.Fecha,
                    Estado = d.Estado,
                    IdTramoHorario = d.IdTramoHorario
                })
                .FirstOrDefaultAsync();
        }




        public async Task<List<DisponibilidadDTO>> GetByIdPuestoTrabajoAsync(int id)
        {
            var disponibilidades = await _context.Disponibilidades
                .Where(d => d.IdPuestoTrabajo == id)
                .Select(d => new DisponibilidadDTO
                {
                    IdDisponibilidad = d.IdDisponibilidad,
                    Fecha = d.Fecha,
                    Estado = d.Estado,
                    IdTramoHorario = d.IdTramoHorario
                })
                .ToListAsync();

            return disponibilidades;
        }



// para reservas
public async Task UpdateDisponibilidadAsync(DisponibilidadDTO disponibilidad)
{
    var disponibilidadObjeto = await _context.Disponibilidades
        .FirstOrDefaultAsync(d => d.IdDisponibilidad == disponibilidad.IdDisponibilidad);

    if (disponibilidadObjeto != null)
    {
        disponibilidadObjeto.Estado = false;
        await _context.SaveChangesAsync();
    }
}


        public async Task AddDisponibilidadesAsync(int anio)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                DateTime fechaIncio = new DateTime(anio, 1, 1); // pendiente filtrado en festivos
                DateTime fechaFin = new DateTime(anio, 12, 31);

                for (DateTime fechaIncremental = fechaIncio; fechaIncremental <= fechaFin; fechaIncremental = fechaIncremental.AddDays(1)) // del 1 de enero al 31 de diciembre sumando 1 dia cada vez
                {
                    string queryInsert = "INSERT INTO Disponibilidades (Fecha, Estado, IdPuestoTrabajo, IdTramoHorario ) VALUES (@Fecha, @Estado, @IdPuestoTrabajo, @IdTramoHorario)";

                    using (var commandInsert = new SqlCommand(queryInsert, connection))
                    {
                        commandInsert.Parameters.AddWithValue("@Fecha", fechaIncremental.Date);
                        commandInsert.Parameters.AddWithValue("@Estado", true); // Siempre será true de inicio
                        commandInsert.Parameters.AddWithValue("@IdPuestoTrabajo", 1);      // Datos de prueba, luego se dinamizaran en un bucle autoincremental
                        commandInsert.Parameters.AddWithValue("@IdTramoHorario", 1);       // Datos de prueba

                        await commandInsert.ExecuteNonQueryAsync();
                    }
                }
            }
        }


    }
}