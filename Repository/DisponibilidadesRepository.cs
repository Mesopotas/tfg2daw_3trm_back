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

            // Obtener los IdPuestoTrabajo disponibles para los ids de los puestos de trabajo
    var puestosTrabajo = await _context.PuestosTrabajo.ToListAsync();

        // Obtener ya existentes en BBDD para evitar duplicados
    var existentes = await _context.Disponibilidades
        .Where(d => d.Fecha.Year == anio && d.IdTramoHorario >= 1 && d.IdTramoHorario <= 11)
        .Select(d => new { d.Fecha, d.IdPuestoTrabajo, d.IdTramoHorario })
        .ToListAsync();

    // crear nuevas disponibilidades
    var nuevasDisponibilidades = new List<Disponibilidad>();

    // recorrer cada puesto de trabajo
    foreach (var puesto in puestosTrabajo)
    {
        // recorrer cada dia laborable
        foreach (var fecha in diasLaborables)
        {
            // recorrer cada tramo horario (de 1 a 11) ya que habrá 11 tramos predefinidos en la bbdd al momento de crearla, por lo tanto 11 ids del 1 al 11
            for (int i = 1; i <= 11; i++)
            {
                // checkear que no exista para evitar repetidos
                if (!existentes.Any(d => d.Fecha.Date == fecha.Date && d.IdPuestoTrabajo == puesto.IdPuestoTrabajo && d.IdTramoHorario == i))
                {
                    // Crear nueva disponibilidad
                    nuevasDisponibilidades.Add(new Disponibilidad
                    {
                        Fecha = fecha,
                        Estado = true, // siempre estarán disponibles al crearlo
                        IdTramoHorario = i,
                        IdPuestoTrabajo = puesto.IdPuestoTrabajo // será el id del puesto de trabajo que estamos recorriendo dentro del foreach
                    });
                }
            }
        }
    }

    // Si hay nuevas disponibilidades, se añaden
    if (nuevasDisponibilidades.Any())
    {
        await _context.Disponibilidades.AddRangeAsync(nuevasDisponibilidades);
        await _context.SaveChangesAsync();
    }
}
    }


}