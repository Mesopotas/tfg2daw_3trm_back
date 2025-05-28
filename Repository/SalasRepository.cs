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


        public async Task<List<SalasDTO>> GetAllAsync()
        {
            return await _context.Salas
                .Select(u => new SalasDTO
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
        }

        public async Task<SalasDTO?> GetByIdAsync(int id)
        {
            return await _context.Salas
                .Where(u => u.IdSala == id)
                .Select(u => new SalasDTO
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
        }

        public async Task AddAsync(SalasDTO sala)
        {
            var entidad = new Salas
            {
                IdSala = sala.IdSala,
                Nombre = sala.Nombre,
                URL_Imagen = sala.URL_Imagen,
                Capacidad = sala.Capacidad,
                IdTipoSala = sala.IdTipoSala,
                IdSede = sala.IdSede,
                Bloqueado = sala.Bloqueado,
            };

            await _context.Salas.AddAsync(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SalasDTO>> GetByIdSedeAsync(int idSede)
        {
            return await _context.Salas
                .Where(s => s.IdSede == idSede)
                .Select(s => new SalasDTO
                {
                    IdSala = s.IdSala,
                    Nombre = s.Nombre,
                    URL_Imagen = s.URL_Imagen,
                    Capacidad = s.Capacidad,
                    IdTipoSala = s.IdTipoSala,
                    IdSede = s.IdSede,
                    Bloqueado = s.Bloqueado
                })
                .ToListAsync();
        }


        public async Task UpdateAsync(SalasDTO sala)
        {
            var entidad = await _context.Salas.FindAsync(sala.IdSala);
            if (entidad != null)
            {
                entidad.Nombre = sala.Nombre;
                entidad.URL_Imagen = sala.URL_Imagen;
                entidad.Capacidad = sala.Capacidad;
                entidad.IdTipoSala = sala.IdTipoSala;
                entidad.IdSede = sala.IdSede;
                entidad.Bloqueado = sala.Bloqueado;

                _context.Salas.Update(entidad);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala != null)
            {

                //  Características de la sala
                var caracteristicas = _context.SalaConCaracteristicas.Where(c => c.IdSala == id);
                _context.SalaConCaracteristicas.RemoveRange(caracteristicas);

                //  Zonas de trabajo
                var zonas = _context.ZonasTrabajo.Where(z => z.IdSala == id).ToList();

                // Eliminar puestos de trabajo de cada zona
                foreach (var zona in zonas)
                {
                    var puestos = _context.PuestosTrabajo.Where(p => p.IdZonaTrabajo == zona.IdZonaTrabajo).ToList();

                    //  Eliminar disponibilidades asociadas a los puestos
                    foreach (var puesto in puestos)
                    {
                        var disponibilidades = _context.Disponibilidades.Where(d => d.IdPuestoTrabajo == puesto.IdPuestoTrabajo);
                        _context.Disponibilidades.RemoveRange(disponibilidades);

                        var lineas = _context.Lineas.Where(l => l.IdPuestoTrabajo == puesto.IdPuestoTrabajo);
                        _context.Lineas.RemoveRange(lineas);
                    }

                    _context.PuestosTrabajo.RemoveRange(puestos);
                }

                // Eliminar zonas
                _context.ZonasTrabajo.RemoveRange(zonas);

                //  Eliminar puestos de trabajo que no tengan zona (si los hay)
                var puestosSinZona = _context.PuestosTrabajo.Where(p => p.IdSala == id && p.IdZonaTrabajo == null);
                foreach (var puesto in puestosSinZona)
                {
                    var disponibilidades = _context.Disponibilidades.Where(d => d.IdPuestoTrabajo == puesto.IdPuestoTrabajo);
                    _context.Disponibilidades.RemoveRange(disponibilidades);

                    var lineas = _context.Lineas.Where(l => l.IdPuestoTrabajo == puesto.IdPuestoTrabajo);
                    _context.Lineas.RemoveRange(lineas);
                }
                _context.PuestosTrabajo.RemoveRange(puestosSinZona);

                // Finalmente, eliminar la sala
                _context.Salas.Remove(sala);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SalasFiltradoDTO>> GetSalasBySede(int idSede, DateTime fechaInicio, DateTime fechaFin, TimeSpan horaInicio, TimeSpan horaFin)
        {
            var query = _context.Salas // selecciona las salas
                  .Where(sala => sala.IdSede == idSede && !sala.Bloqueado) // filtro de sedes y que no sean salas bloqueadas
                  .Join(_context.Sedes, // inner join a sedes
                      sala => sala.IdSede,
                sede => sede.IdSede,
                (sala, sede) => new { sala, sede }) // resultado del join
                  .Select(item => new // proyeccion para calcular info por sala
                  {
                      SalaEntidad = item.sala, // entidad de sala
                      SedeEntidad = item.sede, // entidad de sede

                      // subconsulta para calcular puestos disponibles dentro del rango de fecha/hora
                      PuestosDisponibles = _context.PuestosTrabajo // selecciona puestos
                          .Where(p => p.IdSala == item.sala.IdSala && p.Disponible && !p.Bloqueado) // puestos que sean de la sala, esten como disponibles y no bloqueados
                          .Join(_context.Disponibilidades, // join a disponibilidades
                              puesto => puesto.IdPuestoTrabajo,
                    disp => disp.IdPuestoTrabajo,
                    (puesto, disp) => new { puesto, disp })
                  .Join(_context.TramosHorarios, // join a tramos horarios
                              pd => pd.disp.IdTramoHorario,
                    tramo => tramo.IdTramoHorario,
                    (pd, tramo) => new { pd.puesto, pd.disp, tramo })
                  .Where(filtro => // filtra por disponibilidad, fecha y hora
                              filtro.disp.Estado == true && // disponibilidad activa
                              filtro.disp.Fecha >= fechaInicio && // dentro del rango de fechas (inicio)
                              filtro.disp.Fecha <= fechaFin && // dentro del rango de fechas (fin)
                              filtro.tramo.HoraInicio >= horaInicio && // dentro del rango horario (inicio)
                              filtro.tramo.HoraFin <= horaFin) // dentro del rango horario (fin)
                          .Select(x => x.puesto.IdPuestoTrabajo) // selecciona el id del puesto
                          .Distinct() // ids unicos
                          .Count(), // cuenta los puestos distintos disponibles en el rango

                      // calcula el total de puestos para la sala
                      TotalPuestos = _context.PuestosTrabajo // selecciona puestos
                          .Count(puestos => puestos.IdSala == item.sala.IdSala) // cuenta todos los puestos de la sala
                  })
                  // filtro final: solo incluye salas que tienen al menos un puesto disponible calculado
                  .Where(item => item.PuestosDisponibles > 0) // solo salas con puestos disponibles > 0
                                                              // proyeccion final al dto de resultado
                  .Select(item => new SalasFiltradoDTO // proyeccion final al dto
                  {
                      // propiedades de la sala
                      IdSala = item.SalaEntidad.IdSala,
                      Nombre = item.SalaEntidad.Nombre,
                      URL_Imagen = item.SalaEntidad.URL_Imagen,
                      Capacidad = item.SalaEntidad.Capacidad,
                      IdTipoSala = item.SalaEntidad.IdTipoSala,
                      IdSede = item.SalaEntidad.IdSede,
                      // detalles de la sede
                      SedeObservaciones = item.SedeEntidad.Observaciones,
                      SedePlanta = item.SedeEntidad.Planta,
                      SedeDireccion = item.SedeEntidad.Direccion,
                      SedeCiudad = item.SedeEntidad.Ciudad,
                      // cantidades calculadas
                      PuestosDisponibles = item.PuestosDisponibles, // cantidad de puestos disponibles calculada
                      PuestosOcupados = item.TotalPuestos - item.PuestosDisponibles // cantidad de puestos ocupados calculada
                  });

            return await query.ToListAsync(); // ejecuta la consulta async y retorna la lista
        }


        public async Task<List<SalasConCaracteristicasDTO>> GetAllWithCaracteristicasAsync()
{
    var salasConCaracteristicas = await _context.Salas
        .Select(s => new SalasConCaracteristicasDTO
        {
            IdSala = s.IdSala,
            Nombre = s.Nombre,
            URL_Imagen = s.URL_Imagen,
            Capacidad = s.Capacidad,
            IdTipoSala = s.IdTipoSala,
            IdSede = s.IdSede,
            Bloqueado = s.Bloqueado,
            Caracteristicas = _context.SalaConCaracteristicas
                .Where(sc => sc.IdSala == s.IdSala)
                .Join(_context.CaracteristicasSala,
                    sc => sc.IdCaracteristica,
                    c => c.IdCaracteristica,
                    (sc, c) => new CaracteristicaSalaDTO
                    {
                        IdCaracteristica = c.IdCaracteristica,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        PrecioAniadido = c.PrecioAniadido
                    })
                .ToList()
        })
        .ToListAsync();

    return salasConCaracteristicas;
}

//  agregar una característica a una sala
public async Task AddCaracteristicaToSalaAsync(int idSala, int idCaracteristica)
{
    // verificar que la sala existe
    var salaExiste = await _context.Salas.AnyAsync(s => s.IdSala == idSala);
    if (!salaExiste)
    {
        throw new ArgumentException($"No existe una sala con ID {idSala}");
    }

    // verificar que la característica existe
    var caracteristicaExiste = await _context.CaracteristicasSala.AnyAsync(c => c.IdCaracteristica == idCaracteristica);
    if (!caracteristicaExiste)
    {
        throw new ArgumentException($"No existe una característica con ID {idCaracteristica}");
    }

    // vrificar que la relación no existe ya
    var relacionExiste = await _context.SalaConCaracteristicas
        .AnyAsync(sc => sc.IdSala == idSala && sc.IdCaracteristica == idCaracteristica);
    
    if (relacionExiste)
    {
        throw new InvalidOperationException($"La sala {idSala} ya tiene asignada la característica {idCaracteristica}");
    }

    // crear la relación
    var salaConCaracteristica = new SalaConCaracteristicas
    {
        IdSala = idSala,
        IdCaracteristica = idCaracteristica
    };

    await _context.SalaConCaracteristicas.AddAsync(salaConCaracteristica);
    await _context.SaveChangesAsync();
}

// metodo para eliminar una característica de una sala
public async Task RemoveCaracteristicaFromSalaAsync(int idSala, int idCaracteristica)
{
    // buscar la relación existente
    var salaConCaracteristica = await _context.SalaConCaracteristicas
        .FirstOrDefaultAsync(sc => sc.IdSala == idSala && sc.IdCaracteristica == idCaracteristica);

    if (salaConCaracteristica == null)
    {
        throw new ArgumentException($"No existe una relación entre la sala {idSala} y la característica {idCaracteristica}");
    }

    // eliminar la relación
    _context.SalaConCaracteristicas.Remove(salaConCaracteristica);
    await _context.SaveChangesAsync();
}

// metodo para obtener las características de una sala específica
public async Task<List<CaracteristicaSalaDTO>> GetCaracteristicasBySalaAsync(int idSala)
{
    return await _context.SalaConCaracteristicas
        .Where(sc => sc.IdSala == idSala)
        .Join(_context.CaracteristicasSala,
            sc => sc.IdCaracteristica,
            c => c.IdCaracteristica,
            (sc, c) => new CaracteristicaSalaDTO
            {
                IdCaracteristica = c.IdCaracteristica,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                PrecioAniadido = c.PrecioAniadido
            })
        .ToListAsync();
}
public async Task<string?> GetSalaNameByPuestoTrabajoIdAsync(int idPuestoTrabajo)
{
    // Find the PuestoTrabajo first
    var puesto = await _context.PuestosTrabajo
        .Where(p => p.IdPuestoTrabajo == idPuestoTrabajo)
        .FirstOrDefaultAsync();

    if (puesto == null)
    {
        return null; // no existe el puesto, devuelve nulo
    }

    // buscar sala por IdSala del puesto
    var sala = await _context.Salas
        .Where(s => s.IdSala == puesto.IdSala)
        .Select(s => s.Nombre) // solo el nombre
        .FirstOrDefaultAsync();

    return sala;
}
    }
}

