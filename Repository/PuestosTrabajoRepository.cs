using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;
using Dtos;


namespace CoWorking.Repositories
{
    public class PuestosTrabajoRepository : IPuestosTrabajoRepository
    {
        private readonly CoworkingDBContext _context;


        public PuestosTrabajoRepository(CoworkingDBContext context) // referencia al data.CoworkingDBContext.cs en lugar de cadena de conexión, el EF hará las sentencias sin ponerlas explicitamente
        {
            _context = context;
        }


        public async Task<List<PuestosTrabajoDTO>> GetAllAsync()
        {
            return await _context.PuestosTrabajo
                .Select(p => new PuestosTrabajoDTO
                {
                    IdPuestoTrabajo = p.IdPuestoTrabajo,
                    NumeroAsiento = p.NumeroAsiento,
                    CodigoMesa = p.CodigoMesa,
                    URL_Imagen = p.URL_Imagen,
                    Disponible = p.Disponible,
                    Bloqueado = p.Bloqueado,
                    IdZonaTrabajo = p.IdZonaTrabajo,
                    IdSala = p.IdSala
                })
                .ToListAsync();
        }


        public async Task<PuestosTrabajoDTO> GetByIdAsync(int id)
        {
            return await _context.PuestosTrabajo
                .Where(p => p.IdPuestoTrabajo == id)
                .Select(p => new PuestosTrabajoDTO
                {
                    IdPuestoTrabajo = p.IdPuestoTrabajo,
                    NumeroAsiento = p.NumeroAsiento,
                    CodigoMesa = p.CodigoMesa,
                    URL_Imagen = p.URL_Imagen,
                    Disponible = p.Disponible,
                    Bloqueado = p.Bloqueado,
                    IdZonaTrabajo = p.IdZonaTrabajo,
                    IdSala = p.IdSala
                })
                .FirstOrDefaultAsync();
        }


        public async Task AddAsync(PuestosTrabajoDTO puestoTrabajo)
        {
            var entity = new PuestosTrabajo
            {
                NumeroAsiento = puestoTrabajo.NumeroAsiento,
                CodigoMesa = puestoTrabajo.CodigoMesa,
                URL_Imagen = puestoTrabajo.URL_Imagen,
                Disponible = puestoTrabajo.Disponible,
                Bloqueado = puestoTrabajo.Bloqueado,
                IdZonaTrabajo = puestoTrabajo.IdZonaTrabajo,
                IdSala = puestoTrabajo.IdSala
            };

            await _context.PuestosTrabajo.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PuestosTrabajoDTO puestoTrabajo)
        {
            var puesto = await _context.PuestosTrabajo.FindAsync(puestoTrabajo.IdPuestoTrabajo);

            if (puesto == null)
                throw new Exception($"No se encontró el puesto con ID {puestoTrabajo.IdPuestoTrabajo}.");

            puesto.NumeroAsiento = puestoTrabajo.NumeroAsiento;
            puesto.CodigoMesa = puestoTrabajo.CodigoMesa;
            puesto.URL_Imagen = puestoTrabajo.URL_Imagen;
            puesto.Disponible = puestoTrabajo.Disponible;
            puesto.Bloqueado = puestoTrabajo.Bloqueado;
            puesto.IdZonaTrabajo = puestoTrabajo.IdZonaTrabajo;
            puesto.IdSala = puestoTrabajo.IdSala;

            _context.PuestosTrabajo.Update(puesto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var puesto = await _context.PuestosTrabajo.FindAsync(id);

            if (puesto == null)
                throw new Exception("No se encontró ese puesto de trabajo");

            // Eliminar las disponibilidades asociadas con este puesto de trabajo
            var disponibilidades = _context.Disponibilidades.Where(d => d.IdPuestoTrabajo == id);
            _context.Disponibilidades.RemoveRange(disponibilidades); // elimina todas las disponibilidades asociadas con FK

            var lineas = _context.Lineas.Where(l => l.IdPuestoTrabajo == id);
            _context.Lineas.RemoveRange(lineas); // elimina todas las lineas asociadas con FK

            _context.PuestosTrabajo.Remove(puesto); // elimina el puesto de trabajo
            await _context.SaveChangesAsync();
        }
public async Task<List<PuestoTrabajoFiltroFechasDTO>> GetPuestosWithAvailabilityBySalaAsync(
    int idSala,
    DateTime fechaInicio,
    DateTime fechaFin,
    TimeSpan horaInicio,
    TimeSpan horaFin)
{
    var query = _context.PuestosTrabajo
        .Where(puesto => puesto.IdSala == idSala && !puesto.Bloqueado) // Filtra por la sala especificada y que no haya bloqueados (el admin es quien los bloquea)
        .Join(_context.Salas, // JOIN con Salas para obtener IdTipoSala
            puesto => puesto.IdSala,
            sala => sala.IdSala,
            (puesto, sala) => new { puesto, sala })
        .Join(_context.TiposSalas, // JOIN con TiposSalas para obtener IdTipoPuestoTrabajo
            ps => ps.sala.IdTipoSala,
            tipoSala => tipoSala.IdTipoSala,
            (ps, tipoSala) => new { ps.puesto, ps.sala, tipoSala })
        .Select(pst => new // select del puesto ya filtrado con sus tramos y disponibilidades
        {
            PuestoEntidad = pst.puesto,
            IdTipoPuestoTrabajo = pst.tipoSala.IdTipoPuestoTrabajo, // obtener IdTipoPuestoTrabajo
            DisponibilidadesEnRango = _context.Disponibilidades
                .Where(disp => disp.IdPuestoTrabajo == pst.puesto.IdPuestoTrabajo)
                .Join(_context.TramosHorarios,
                    disp => disp.IdTramoHorario,
                    tramo => tramo.IdTramoHorario,
                    (disp, tramo) => new { disp, tramo })
                .Where(pd =>
                    pd.disp.Fecha >= fechaInicio && pd.disp.Fecha <= fechaFin &&
                    pd.tramo.HoraInicio >= horaInicio && pd.tramo.HoraFin <= horaFin) // filtro horario
                .Select(pd => new DisponibilidadFiltroFechasDTO // array de disponibilidades que cumplen los filtros
                {
                    IdDisponibilidad = pd.disp.IdDisponibilidad,
                    Fecha = pd.disp.Fecha,
                    Estado = pd.disp.Estado,
                    IdTramoHorario = pd.tramo.IdTramoHorario,
                    HoraInicio = pd.tramo.HoraInicio,
                    HoraFin = pd.tramo.HoraFin
                })
                .ToList() // se ejecuta y almacena en el array los resultados obtenidos
        })
        .Where(item => item.DisponibilidadesEnRango.Any()) // filtro, solo sacará los resultados que tengan al menos 1 disponibilidad en el rango de fecha-hora, si no, no saldrá
        .Select(item => new PuestoTrabajoFiltroFechasDTO // resultado final del array list con la data del dto
        {
            IdPuestoTrabajo = item.PuestoEntidad.IdPuestoTrabajo,
            NumeroAsiento = item.PuestoEntidad.NumeroAsiento,
            CodigoMesa = item.PuestoEntidad.CodigoMesa,
            URL_Imagen = item.PuestoEntidad.URL_Imagen,
            DisponibleGeneral = item.DisponibilidadesEnRango.All(d => d.Estado), // true solo si TODAS las disponibilidades tienen estado=true (100% disponible)
            BloqueadoGeneral = item.PuestoEntidad.Bloqueado, // lo mismo q arriba, serian las que bloquease el admin, no tiene que ver con la disponibilidad
            IdZonaTrabajo = item.PuestoEntidad.IdZonaTrabajo,
            IdSala = item.PuestoEntidad.IdSala,
            IdTipoPuestoTrabajo = item.IdTipoPuestoTrabajo,
            DisponibilidadesEnRango = item.DisponibilidadesEnRango // array de disponibilidades que cumplen los filtros
        });

    return await query.ToListAsync(); // devuelve el listado de asientos, cada asiento con sus disponibilidades acorde al filtro
}

public async Task GenerarAsientosDeSalas()
        {
            var salas = await _context.Salas
            // para acceder a las capacidades de asientos y mesas
                                    .Include(s => s.TipoSala) 
                                    .Include(s => s.ZonasTrabajo)
                                    .ToListAsync();

            foreach (var sala in salas)
            {
                var puestosExistentes = await _context.PuestosTrabajo
                                                        .Where(p => p.IdSala == sala.IdSala)
                                                        .AnyAsync();

                if (!puestosExistentes)
                {
                    if (sala.TipoSala != null) // comprobar que exista
                    {
                        var capacidadSala = sala.TipoSala.CapacidadAsientos;
                        var idZonaTrabajo = sala.ZonasTrabajo.FirstOrDefault()?.IdZonaTrabajo;

                        if (idZonaTrabajo == null)
                        {
                            var nuevaZona = new ZonasTrabajo // generar zonas de trabajo por defecto
                            {
                                Descripcion = $"Zona de trabajo por defecto para {sala.Nombre}",
                                IdSala = sala.IdSala
                            };
                            _context.ZonasTrabajo.Add(nuevaZona);
                            await _context.SaveChangesAsync();
                            idZonaTrabajo = nuevaZona.IdZonaTrabajo;
                        }

                        for (int i = 1; i <= capacidadSala; i++)
                        {
                            var nuevoPuesto = new PuestosTrabajo
                            {
                                NumeroAsiento = i,
                                CodigoMesa = sala.TipoSala.NumeroMesas > 0 ? (i - 1) / (capacidadSala / sala.TipoSala.NumeroMesas) + 1 : 1, /* asigna un número de mesa (CodigoMesa) a cada asiento equitativamente entre el número de mesas de la sala.
                                 Si la sala no tiene mesas, le asigna la mesa 1 por defecto.*/
                                URL_Imagen = "imagen.png", // cambiar a la imagen que se vaya a usar
                                Disponible = true,
                                Bloqueado = false,
                                IdZonaTrabajo = idZonaTrabajo.Value,
                                IdSala = sala.IdSala
                            };
                            await _context.PuestosTrabajo.AddAsync(nuevoPuesto);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Advertencia: Sala '{sala.Nombre}' (ID: {sala.IdSala}) no tiene un TipoSala asociado cargado. Los asientos no se crearán para esta sala.");
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
    