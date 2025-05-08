using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using System.Data;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;
using Nager.Date;


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
            var fechaInicio = new DateTime(anio, 1, 1); // dia 1 de enero
            var fechaFin = new DateTime(anio, 12, 31); // dia 31 de diciembre

            // dotnet add package Nager.Date --version 1.28.0 IMPORTANTE la version, en algunas anteriores no admite el comando, que sea esa siempre
            // Obtener festivos nacionales en España con la libreria Nager.Date
            var festivosEspania = DateSystem.GetPublicHoliday(anio, "ES")
                .Select(festivo => festivo.Date)
             .ToList();

            // Generar lista de días laborables (lunes a viernes, que no sean festivos)
            var diasLaborables = Enumerable.Range(0, (fechaFin - fechaInicio).Days + 1) // resta el fechaFin al fechaInicio y le suma 1 para incluir el ultimo dia, ya se tiene el listado de los dias
                .Select(secuenciaDias => fechaInicio.AddDays(secuenciaDias)) // transforma ese listado en un listado de fechas
                .Where(fecha =>
                    fecha.DayOfWeek != DayOfWeek.Saturday && // DayOfWeek es una propiedad de DateTime que devuelve el dia de la semana, en este caso que sea diferente del sabado 
                    fecha.DayOfWeek != DayOfWeek.Sunday && // diferente del domingo
                    !festivosEspania.Contains(fecha.Date)) // que no sea festivo, de la lista previamente obtenida arriba con Nager.Date
                .ToList();

            // Obtener ya existentes en BBDD para evitar duplicados
            var existentes = await _context.Disponibilidades
                .Where(d => d.Fecha.Year == anio && d.IdTramoHorario == 1 && d.IdPuestoTrabajo == 1)
                .Select(d => d.Fecha)
             .ToListAsync();

            // Crear solo los que no existen
            var nuevasDisponibilidades = diasLaborables
                .Where(fecha => !existentes.Contains(fecha))
                .Select(fecha => new Disponibilidad
                {
                    Fecha = fecha,
                    Estado = true,
                    IdTramoHorario = 1,    // luego se dinamizará con un bucle
                    IdPuestoTrabajo = 6
                })
                .ToList();

            if (nuevasDisponibilidades.Any()) // si hay 1 o mas cambios por añadir, ejecutará su insert
            {
                await _context.Disponibilidades.AddRangeAsync(nuevasDisponibilidades);
                await _context.SaveChangesAsync();
            }
        }

    }


}
