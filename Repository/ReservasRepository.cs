using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;


namespace CoWorking.Repositories
{
    public class ReservasRepository : IReservasRepository
    {
        private readonly CoworkingDBContext _context;

        public ReservasRepository(CoworkingDBContext context)
        {
            _context = context;
        }

        public async Task<List<ReservasDTO>> GetAllAsync()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Usuario)
                .Select(r => new ReservasDTO
                {
                    IdReserva = r.IdReserva,
                    Fecha = r.Fecha,
                    ReservaDescripcion = r.Descripcion,
                    PrecioTotal = r.PrecioTotal,
                    UsuarioId = r.Usuario.IdUsuario,
                    UsuarioNombre = r.Usuario.Nombre,
                    UsuarioEmail = r.Usuario.Email
                })
                .ToListAsync();

            return reservas;
        }

        public async Task<ReservasDTO?> GetByIdAsync(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .Where(r => r.IdReserva == id)
                .Select(r => new ReservasDTO
                {
                    IdReserva = r.IdReserva,
                    Fecha = r.Fecha,
                    ReservaDescripcion = r.Descripcion,
                    PrecioTotal = r.PrecioTotal,
                    UsuarioId = r.Usuario.IdUsuario,
                    UsuarioNombre = r.Usuario.Nombre,
                    UsuarioEmail = r.Usuario.Email
                })
                .FirstOrDefaultAsync();

            return reserva;
        }
        public async Task<Reservas> CreateReservaAsync(Reservas reserva)
        {
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            return reserva;
        }

      public async Task<List<GetReservasClienteDTO>> GetReservasUsuarioAsync(int idUsuario)
{
    // sentencia muy optimizada para velocidad
    var resultado = await _context.Reservas
        .Where(r => r.IdUsuario == idUsuario)
        .OrderByDescending(r => r.Fecha)
        .Select(r => new GetReservasClienteDTO
        {
            IdReserva = r.IdReserva,
            PrecioTotal = r.PrecioTotal,
            
            // Info del primer asiento, ya que son los mismos en toda la sala
            NombreSalaPrincipal = r.Lineas.First().PuestoTrabajo.Sala.Nombre,
            CiudadSedePrincipal = r.Lineas.First().PuestoTrabajo.Sala.Sede.Ciudad,
            DireccionSedePrincipal = r.Lineas.First().PuestoTrabajo.Sala.Sede.Direccion,
            ImagenSalaPrincipal = r.Lineas.First().PuestoTrabajo.Sala.URL_Imagen,
            
            AsientosSeleccionados = r.Lineas
                .Select(l => l.PuestoTrabajo.NumeroAsiento)
                .Distinct()
                .ToList(),
            
            // nuevo campo en bbdd, ahorra consultar en disponibilidades sumando muchisimo tiempo
            RangoHorarioReserva = r.TramoReservado ?? r.Fecha.ToString("dd/MM/yyyy HH:mm")
        })
        .ToListAsync();

    return resultado;
}
public async Task<Reservas> CreateReservaConLineasAsync(ReservaPostDTO reservaDTO)
{
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        try
        {
            // Variables para calcular el tramo reservado
            DateTime? fechaInicioMin = null;
            DateTime? fechaFinMax = null;

            var reserva = new Reservas
            {
                IdUsuario = reservaDTO.IdUsuario,
                Descripcion = reservaDTO.Descripcion,
                Fecha = reservaDTO.FechaReserva,
                PrecioTotal = 0
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            decimal precioTotal = 0;
            var disponibilidadesAProcesar = new List<LineasPostReservaDTO>();

            if (reservaDTO.Lineas != null && reservaDTO.Lineas.Any())
            {
                var gruposPorPuestoTrabajo = reservaDTO.Lineas
                    .GroupBy(l => l.IdPuestoTrabajo);

                foreach (var grupo in gruposPorPuestoTrabajo)
                {
                    var idPuestoTrabajo = grupo.Key;
                    var lineasDelGrupo = grupo.OrderBy(l => l.IdDisponibilidad).ToList();

                    if (lineasDelGrupo.Count >= 2)
                    {
                        var idDisponibilidadInicio = lineasDelGrupo.First().IdDisponibilidad;
                        var idDisponibilidadFin = lineasDelGrupo.Last().IdDisponibilidad;

                        // obtener las disponibilidades y ordenarlas por fecha y hora
                        var disponibilidadesEnRango = await _context.Disponibilidades
                            .Include(d => d.TramoHorario)
                            .Where(d => d.IdPuestoTrabajo == idPuestoTrabajo &&
                                        d.IdDisponibilidad >= idDisponibilidadInicio &&
                                        d.IdDisponibilidad <= idDisponibilidadFin)
                            .OrderBy(d => d.Fecha)
                            .ThenBy(d => d.TramoHorario.HoraInicio)
                            .ToListAsync();

                        foreach (var disp in disponibilidadesEnRango)
                        {
                            disponibilidadesAProcesar.Add(new LineasPostReservaDTO
                            {
                                IdPuestoTrabajo = disp.IdPuestoTrabajo,
                                IdDisponibilidad = disp.IdDisponibilidad
                            });
                        }
                    }
                    else if (lineasDelGrupo.Count == 1)
                    {
                        disponibilidadesAProcesar.Add(lineasDelGrupo.Single());
                    }
                }
            }

            // procesar todas las disponibilidades y calcular el tramo
            var todasLasDisponibilidades = new List<Disponibilidad>();

            foreach (var lineaDTO in disponibilidadesAProcesar)
            {
                var disponibilidad = await _context.Disponibilidades
                    .Include(d => d.PuestoTrabajo)
                        .ThenInclude(p => p.Sala)
                            .ThenInclude(s => s.TipoSala)
                                .ThenInclude(ts => ts.TipoPuestoTrabajo)
                    .Include(d => d.TramoHorario)
                    .FirstOrDefaultAsync(d => d.IdDisponibilidad == lineaDTO.IdDisponibilidad);

                if (disponibilidad == null)
                {
                    throw new ArgumentException($"No existe la disponibilidad con Id: {lineaDTO.IdDisponibilidad}");
                }

                if (!disponibilidad.Estado)
                {
                    throw new InvalidOperationException($"El puesto de trabajo asociado a la disponibilidad {lineaDTO.IdDisponibilidad} no está disponible.");
                }

                // añadir a la lista para calcular el tramo total
                todasLasDisponibilidades.Add(disponibilidad);

                var puestoTrabajo = disponibilidad.PuestoTrabajo;
                if (puestoTrabajo == null)
                {
                    throw new ArgumentException($"La disponibilidad con Id: {lineaDTO.IdDisponibilidad} no tiene un puesto de trabajo asociado válido.");
                }

                decimal precioLinea = 0;
                if (puestoTrabajo.Sala?.TipoSala?.TipoPuestoTrabajo != null)
                {
                    precioLinea = puestoTrabajo.Sala.TipoSala.TipoPuestoTrabajo.Precio;
                }
                else
                {
                    throw new InvalidOperationException($"No se pudo determinar el precio base para el puesto de trabajo {puestoTrabajo.IdPuestoTrabajo}. Faltan datos de Sala/TipoSala/TipoPuestoTrabajo.");
                }

                var caracteristicasSala = await _context.SalaConCaracteristicas
                    .Include(sc => sc.Caracteristica)
                    .Where(sc => sc.IdSala == puestoTrabajo.IdSala)
                    .ToListAsync();

                foreach (var caracteristica in caracteristicasSala)
                {
                    precioLinea += precioLinea * (caracteristica.Caracteristica.PrecioAniadido / 100m);
                }

                var linea = new Lineas
                {
                    IdReserva = reserva.IdReserva,
                    IdPuestoTrabajo = puestoTrabajo.IdPuestoTrabajo,
                    Precio = precioLinea
                };

                _context.Lineas.Add(linea);
                disponibilidad.Estado = false;
                precioTotal += precioLinea;
            }

            // calcular tramo
            if (todasLasDisponibilidades.Any())
            {
                // ordenar todas las disponibilidades por fecha y hora
                var disponibilidadesOrdenadas = todasLasDisponibilidades
                    .Where(d => d.TramoHorario != null)
                    .OrderBy(d => d.Fecha)
                    .ThenBy(d => d.TramoHorario.HoraInicio)
                    .ToList();

                if (disponibilidadesOrdenadas.Any())
                {
                    var primera = disponibilidadesOrdenadas.First();
                    var ultima = disponibilidadesOrdenadas.Last();

                    fechaInicioMin = primera.Fecha.Date.Add(primera.TramoHorario.HoraInicio);
                    fechaFinMax = ultima.Fecha.Date.Add(ultima.TramoHorario.HoraFin);

                    reserva.TramoReservado = $"{fechaInicioMin.Value:dd/MM/yyyy HH:mm} - {fechaFinMax.Value:dd/MM/yyyy HH:mm}";
                    
                }
            }

            reserva.PrecioTotal = precioTotal;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return reserva;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
        public async Task UpdateAsync(ReservasUpdateDTO reservas)
        {
            var reservaExistente = await _context.Reservas
                .FirstOrDefaultAsync(r => r.IdReserva == reservas.IdReserva);

            if (reservaExistente == null)
                throw new InvalidOperationException("Reserva no encontrada");

            reservaExistente.Fecha = reservas.Fecha;
            reservaExistente.PrecioTotal = reservas.PrecioTotal;
            reservaExistente.Descripcion = reservas.Descripcion;
            reservaExistente.IdUsuario = reservas.IdUsuario;

            await _context.SaveChangesAsync();
        }




          public async Task DeleteAsync(int id)
        {
            var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva != null) // si la reserva existe, sino no se hace nada
            {
                // obtener todas las lineas asociadas a la reserva
                var lineas = await _context.Lineas
                    .Where(l => l.IdReserva == id)
                    .ToListAsync();

                // ids de los puestos de trabajo asociados a las lineas de la reserva
                var puestoTrabajoIds = lineas.Select(l => l.IdPuestoTrabajo).Distinct().ToList();

    
                var disponibilidadesLiberar = await _context.Disponibilidades // las disponibilidades que tenia la reserva, se ponen libres (Estado = true)
                    .Where(d => puestoTrabajoIds.Contains(d.IdPuestoTrabajo) &&
                                d.Fecha.Date == reserva.Fecha.Date &&
                                d.Estado == false)
                                        //    disponibilidades que estaban asociadas a los puestos de trabajo de la reserva, en su fecha y que estaban ocupadas

                    .ToListAsync();

                foreach (var disponibles in disponibilidadesLiberar)
                {
                    disponibles.Estado = true;
                    _context.Disponibilidades.Update(disponibles); // guardr todos como disponibles
                }

                _context.Lineas.RemoveRange(lineas); // borrar tosdas las lineas asociadas a la reserva

                // eliminar la reserva
                _context.Reservas.Remove(reserva);

                // guardar todo
                await _context.SaveChangesAsync();
            }
        }
  
public async Task<GetDetallesReservaDTO?> GetResumenReservaAsync(int idReserva)
{
    // obtener la reserva y sus líneas asociadas
    var reservaConLineasYDetalles = await _context.Reservas
        .Where(r => r.IdReserva == idReserva) // filtro por ID
        .Select(r => new
        {
            Reserva = r,
            LineasDetalladas = r.Lineas
                                    .Select(l => new
                                    {
                                        l.IdPuestoTrabajo,
                                        Sala = l.PuestoTrabajo.Sala, // obtener salas
                                        Sede = l.PuestoTrabajo.Sala.Sede  // obtener sedes en base a sala
                                    })
                                    .ToList() // añadir respuesta
        })
        .FirstOrDefaultAsync(); // primera reserva que coincida con todo

    // no no se encuentra la reserva, devolver nulo
    if (reservaConLineasYDetalles == null)
    {
        return null;
    }

    var reserva = reservaConLineasYDetalles.Reserva;
    var lineasDetalladas = reservaConLineasYDetalles.LineasDetalladas;

    // obtener los ids de los puestos de la linea sin que se repitan
    var puestoTrabajoIds = lineasDetalladas.Select(ld => ld.IdPuestoTrabajo).Distinct().ToList();

    // obtener info de los puestos
    var puestosTrabajo = await _context.PuestosTrabajo
        .Where(pt => puestoTrabajoIds.Contains(pt.IdPuestoTrabajo))
        .Select(pt => new { pt.IdPuestoTrabajo, pt.NumeroAsiento, pt.CodigoMesa })
        .ToListAsync();

    // formatear el output del string de asientos, separandolos por comas y dandoles formato legible
    var asientosReservados = string.Join(", ", 
        puestosTrabajo
            .OrderBy(pt => pt.CodigoMesa)
            .ThenBy(pt => pt.NumeroAsiento)
            .Select(pt => $"Mesa {pt.CodigoMesa} - Asiento {pt.NumeroAsiento}"));

    // si no hay asientos que encuentre, solo cantidad, aunque no deberia suceder nunca
    if (string.IsNullOrEmpty(asientosReservados))
    {
        asientosReservados = $"{puestoTrabajoIds.Count} puesto(s) de trabajo";
    }

    // misma logica que GetReservasUsuarioAsync
    var disponibilidadesAsociadas = await _context.Disponibilidades
        .Where(d => puestoTrabajoIds.Contains(d.IdPuestoTrabajo) &&
                    d.Estado == false &&
                    d.Fecha.Date >= reserva.Fecha.Date)
        .Include(d => d.TramoHorario)
        .OrderBy(d => d.Fecha) // primero se ordena por fechas
        .ThenBy(d => d.TramoHorario.HoraInicio) // luego por horas
        .ToListAsync();

    // horas reservas el conteo de disponibilidades que haya
    int cantidadHorasReservadas = disponibilidadesAsociadas.Count;

    // igual que GetReservasUsuarioAsync para dar con la fecha y hora de inicio y fin
    string rangoHorario = "Horario no disponible";
    if (disponibilidadesAsociadas.Any())
    {
        // fecha de inicio y fin (mas grande y mas pequeña)
        DateTime minFecha = disponibilidadesAsociadas.Min(d => d.Fecha);
        DateTime maxFecha = disponibilidadesAsociadas.Max(d => d.Fecha);

        // hora de inicio
        TimeSpan minHoraInicio = disponibilidadesAsociadas
                                    .Where(d => d.Fecha == minFecha)
                                    .Min(d => d.TramoHorario.HoraInicio);

        //  hora de fin
        TimeSpan maxHoraFin = disponibilidadesAsociadas
                                    .Where(d => d.Fecha == maxFecha)
                                    .Max(d => d.TramoHorario.HoraFin);

        if (minFecha.Date == maxFecha.Date)
        {
            // formato legible para 1 dia solo
            rangoHorario = $"{minFecha:dd/MM/yyyy} {minHoraInicio:hh\\:mm} - {maxHoraFin:hh\\:mm}";
        }
        else
        {
            // formato legible para varios dias
            rangoHorario = $"{minFecha:dd/MM/yyyy} {minHoraInicio:hh\\:mm} - {maxFecha:dd/MM/yyyy} {maxHoraFin:hh\\:mm}";
        }
    }
    else if (lineasDetalladas.Any())
    {
        // si no hay disponibilidades, pero si lineas, se pone el rango de la reserva, aunque no deberia suceder en ningun caso
        rangoHorario = $"{reserva.Fecha:dd/MM/yyyy} (Horario no especificado)";
    }

    // informacion de sala y sede en base a la primera linea, ya que es todo de la misma sala y sede
    var primeraLineaValida = lineasDetalladas.FirstOrDefault(ld => ld.Sala != null && ld.Sede != null);

    // output final con toda la data
    return new GetDetallesReservaDTO
    {
        IdReserva = reserva.IdReserva,
        PrecioTotal = reserva.PrecioTotal,
        CantidadHorasReservadas = cantidadHorasReservadas,
        
        NombreSalaPrincipal = primeraLineaValida?.Sala?.Nombre,
        CiudadSedePrincipal = primeraLineaValida?.Sede?.Ciudad,
        DireccionSedePrincipal = primeraLineaValida?.Sede?.Direccion,
        RangoHorarioReserva = rangoHorario,
        AsientosReservados = asientosReservados
    };
}
        public async Task<bool> ValidarReservaExisteQR(int idReserva, int idUsuario, DateTime fecha)
        {
            var existe = await _context.Reservas
                // si cumple esos 3 requisitos, existe, si el usuario no es el mismo o la fecha esta mal, devolverá false
                .AnyAsync(r => r.IdReserva == idReserva &&
                               r.IdUsuario == idUsuario &&
                               r.Fecha.Date == fecha.Date);

            return existe;
        }
    }

}