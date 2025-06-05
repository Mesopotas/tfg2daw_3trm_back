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
    // Buscar el TipoSala para obtener su CapacidadAsientos
    var tipoSala = await _context.TiposSalas
        .FirstOrDefaultAsync(ts => ts.IdTipoSala == sala.IdTipoSala);

    if (tipoSala == null)
    {
        throw new ArgumentException($"No existe un tipo de sala con ID {sala.IdTipoSala}");
    }

    var entidad = new Salas
    {
        IdSala = sala.IdSala,
        Nombre = sala.Nombre,
        URL_Imagen = sala.URL_Imagen,
        Capacidad = tipoSala.CapacidadAsientos, // asignar la capacidad total en base a la capacidad del tipo de sala
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
    var query = from sala in _context.Salas
                join sede in _context.Sedes on sala.IdSede equals sede.IdSede
                join puesto in _context.PuestosTrabajo on sala.IdSala equals puesto.IdSala
                join disp in _context.Disponibilidades on puesto.IdPuestoTrabajo equals disp.IdPuestoTrabajo
                join tramo in _context.TramosHorarios on disp.IdTramoHorario equals tramo.IdTramoHorario
                where sala.IdSede == idSede && !sala.Bloqueado &&
                      puesto.Disponible && !puesto.Bloqueado &&
                      disp.Estado == true &&
                      disp.Fecha >= fechaInicio && disp.Fecha <= fechaFin &&
                      tramo.HoraInicio >= horaInicio && tramo.HoraFin <= horaFin
                group puesto by new { 
                    sala.IdSala, sala.Nombre, sala.URL_Imagen, sala.Capacidad, 
                    sala.IdTipoSala, sala.IdSede,
                    sede.Observaciones, sede.Planta, sede.Direccion, sede.Ciudad
                } into g
                select new SalasFiltradoDTO
                {
                    IdSala = g.Key.IdSala,
                    Nombre = g.Key.Nombre,
                    URL_Imagen = g.Key.URL_Imagen,
                    Capacidad = g.Key.Capacidad,
                    IdTipoSala = g.Key.IdTipoSala,
                    IdSede = g.Key.IdSede,
                    SedeObservaciones = g.Key.Observaciones,
                    SedePlanta = g.Key.Planta,
                    SedeDireccion = g.Key.Direccion,
                    SedeCiudad = g.Key.Ciudad,
                    PuestosDisponibles = g.Select(p => p.IdPuestoTrabajo).Distinct().Count(),
                    PuestosOcupados = _context.PuestosTrabajo.Count(p => p.IdSala == g.Key.IdSala) - g.Select(p => p.IdPuestoTrabajo).Distinct().Count()
                };

    return await query.ToListAsync();
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


    public async Task<decimal?> GetPrecioPuestoTrabajoAsync(int idPuestoTrabajo)
        {
            // primero se hacen los joins necesarios, ya que el puesto de trabajo tiene una sala, y la sala tiene un tipo de sala, y el tipo de sala tiene un tipo de puesto de trabajo
            var puestoTrabajo = await _context.PuestosTrabajo
                .Include(p => p.Sala)
                    .ThenInclude(s => s.TipoSala)
                        .ThenInclude(ts => ts.TipoPuestoTrabajo)
                .FirstOrDefaultAsync(p => p.IdPuestoTrabajo == idPuestoTrabajo);

            if (puestoTrabajo == null)
            {
                return null; // el puesto de trabajo no existe
            }

            // obtener el precio base del tipo de puesto de trabajo
            decimal precioBase = 0;
            if (puestoTrabajo.Sala?.TipoSala?.TipoPuestoTrabajo != null)
            {
                precioBase = puestoTrabajo.Sala.TipoSala.TipoPuestoTrabajo.Precio;
            }
            else
            {
                throw new InvalidOperationException($"No se pudo determinar el precio base para el puesto de trabajo {idPuestoTrabajo}. Faltan datos de Sala/TipoSala/TipoPuestoTrabajo.");
            }

            // obtener caracteristicas de la sala en la que esta ese puesto de trabajo
            var caracteristicasSala = await _context.SalaConCaracteristicas
                .Include(sc => sc.Caracteristica)
                .Where(sc => sc.IdSala == puestoTrabajo.IdSala)
                .ToListAsync();

            decimal precioFinal = precioBase;

            // aplicar el precio añadido por cada característica de la sala
            foreach (var caracteristica in caracteristicasSala)
            {
                // el precio añadido se aplica como porcentaje sobre el precio actual
                precioFinal += precioFinal * (caracteristica.Caracteristica.PrecioAniadido / 100m);
            }

            return precioFinal;
        }

    }
}

