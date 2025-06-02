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
    var fechaInicio = (anio == DateTime.Now.Year) ? DateTime.Today : new DateTime(anio, 1, 1);
    var fechaFin = new DateTime(anio, 12, 31);

    // Obtener festivos y días laborables (esto ya está optimizado)
    var festivosEspania = DateSystem.GetPublicHoliday(anio, "ES")
        .Select(festivo => festivo.Date)
        .ToHashSet(); 

    var diasLaborables = Enumerable.Range(0, (fechaFin - fechaInicio).Days + 1)
        .Select(secuenciaDias => fechaInicio.AddDays(secuenciaDias))
        .Where(fecha =>
            fecha.DayOfWeek != DayOfWeek.Saturday &&
            fecha.DayOfWeek != DayOfWeek.Sunday &&
            !festivosEspania.Contains(fecha.Date))
        .ToList();

    // Obtener solo los IDs de puestos (no toda la entidad)
    var idsPuestosTrabajo = await _context.PuestosTrabajo
        .Select(p => p.IdPuestoTrabajo)
        .ToListAsync();

    // Obtener TODOS los existentes de una vez con una sola consulta
    var existentesDelAnio = await _context.Disponibilidades
        .Where(d => d.Fecha.Year == anio && d.IdTramoHorario >= 1 && d.IdTramoHorario <= 11)
        .Select(d => new { d.Fecha.Date, d.IdPuestoTrabajo, d.IdTramoHorario })
        .ToListAsync();

    // Convertir a HashSet para búsquedas O(1) en lugar de O(n)
    var existentesSet = existentesDelAnio
        .Select(d => $"{d.Date:yyyyMMdd}_{d.IdPuestoTrabajo}_{d.IdTramoHorario}")
        .ToHashSet();

    const int tamanoLote = 50000;
    var todasLasDisponibilidades = new List<Disponibilidad>();

    // Pre-generar  disponibilidades en memoria
    foreach (var fecha in diasLaborables)
    {
        foreach (var idPuesto in idsPuestosTrabajo)
        {
            for (int tramoHorario = 1; tramoHorario <= 11; tramoHorario++)
            {
                var clave = $"{fecha:yyyyMMdd}_{idPuesto}_{tramoHorario}";
                
                // Búsqueda O(1) en lugar de O(n)
                if (!existentesSet.Contains(clave))
                {
                    todasLasDisponibilidades.Add(new Disponibilidad
                    {
                        Fecha = fecha,
                        Estado = true,
                        IdTramoHorario = tramoHorario,
                        IdPuestoTrabajo = idPuesto
                    });
                }
            }
        }
    }

    Console.WriteLine($"Generadas {todasLasDisponibilidades.Count} disponibilidades nuevas en memoria");

    // Insertar por lotes grandes
    for (int i = 0; i < todasLasDisponibilidades.Count; i += tamanoLote)
    {
        var lote = todasLasDisponibilidades.Skip(i).Take(tamanoLote).ToList();
        
        try
        {
            _context.Database.SetCommandTimeout(300); // 5 minutos timeout para evitar errores de exceso de tiempo
            
            await _context.Disponibilidades.AddRangeAsync(lote);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Insertado lote {i/tamanoLote + 1}: {lote.Count} registros");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en lote {i/tamanoLote + 1}: {ex.Message}");
            
            // Si falla el lote grande, intentar con lotes más pequeños
            await InsertarLotePequeno(lote);
        }
    }
}

// Método auxiliar para manejar errores con lotes más pequeños
private async Task InsertarLotePequeno(List<Disponibilidad> loteGrande)
{
    const int tamanoLotePequeno = 5000;
    
    for (int i = 0; i < loteGrande.Count; i += tamanoLotePequeno)
    {
        var lotePequeno = loteGrande.Skip(i).Take(tamanoLotePequeno).ToList();
        
        try
        {
            await _context.Disponibilidades.AddRangeAsync(lotePequeno);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Recuperado con lote pequeño: {lotePequeno.Count} registros");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error crítico en lote pequeño: {ex.Message}");
        }
    }
}
public async Task<List<FechasDisponiblesDTO>> GetDiasBySalaAsync(int salaId)
{
    // Consultamos primero las fechas únicas
    var fechasUnicas = await _context.Disponibilidades
        .Where(d => d.Estado &&
                d.PuestoTrabajo.Disponible &&
                !d.PuestoTrabajo.Bloqueado &&
                d.PuestoTrabajo.Sala.IdSala == salaId)
        .Select(d => d.Fecha.Date) // se elige la fecha obtenida de disponibilidad
        .Distinct() // igual que en sql, evita duplicados, ya que cada fecha puede tener varios puestos de trabajo de esa misma sala
        .OrderBy(fecha => fecha) // primero cargará las fechas mas recientes en orden ascendente
        .ToListAsync();
    
    // para cada fecha única, se elige su primera disponibilidad, sino cada fecha aparecería varias veces
    var output = new List<FechasDisponiblesDTO>();
    
    foreach (var fecha in fechasUnicas)
    {
        // select de esa fecha para tener el id del puesto de trabajo al que corresponde y su id de disponibilidad
        var disponibilidad = await _context.Disponibilidades
            .Where(d => d.Estado &&
                    d.PuestoTrabajo.Disponible &&
                    !d.PuestoTrabajo.Bloqueado &&
                    d.PuestoTrabajo.Sala.IdSala == salaId &&
                    d.Fecha.Date == fecha)
            .FirstOrDefaultAsync();
        
        if (disponibilidad != null) // si existe la disponibilidad
        {
            output.Add(new FechasDisponiblesDTO // añade esa info al dato de salida con las propiedades del DTO
            {
                IdDisponibilidad = disponibilidad.IdDisponibilidad,
                Fecha = disponibilidad.Fecha,
                IdPuestoTrabajo = disponibilidad.IdPuestoTrabajo
            });
        }
    }
    
    return output;
}


}
}