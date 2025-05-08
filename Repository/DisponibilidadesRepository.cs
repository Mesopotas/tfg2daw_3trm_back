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
     
    public async Task<List<Disponibilidad>> GetAllAsync()
{
  {
            return await _context.Disponibilidades
            .ToListAsync();
        };
}


        public async Task<Disponibilidad> GetByIdAsync(int id)
     {
            return await _context.Disponibilidades.FirstOrDefaultAsync(disponibilidad => disponibilidad.IdDisponibilidad == id); // funcion flecha, usuario recoge todos los usuarios quer cumple que IdUsuario == id
        }




public async Task<List<DisponibilidadDTO>> GetByIdPuestoTrabajoAsync(int id)
{
    var disponibilidades = await _context.Disponibilidades
        .Where(d => d.IdPuestoTrabajo == id)
        .Select(d => new DisponibilidadDTO
        {
            IdDisponibilidad = d.IdDisponibilidad,
            Estado = d.Estado,
            IdTramoHorario = d.IdTramoHorario
        })
        .ToListAsync();

    return disponibilidades;
}




        public async Task UpdateDisponibilidadAsync(DisponibilidadDTO disponibilidad)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Disponibilidades SET Estado = @Estado WHERE IdDisponibilidad = @IdDisponibilidad";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdDisponibilidad", disponibilidad.IdDisponibilidad);
                    command.Parameters.AddWithValue("@Estado", false);

                    await command.ExecuteNonQueryAsync();
                }
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