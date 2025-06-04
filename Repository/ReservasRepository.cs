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

public async Task<Reservas> CreateReservaConLineasAsync(ReservaPostDTO reservaDTO)
{
    // iniciar una transacción, esto hará que si 2 usuarios intentan comprar una o varias disponibilidades iguales simultaneamente, nunca se crearán registros vacios en la bbdd
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        try
        {
            // crear reserva
            var reserva = new Reservas
            {
                IdUsuario = reservaDTO.IdUsuario,
                Descripcion = reservaDTO.Descripcion,
                Fecha = reservaDTO.FechaReserva,
                PrecioTotal = 0 // inicializado a 0 se actualizara al agregar lineas
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // crear lineas de la reserva
            decimal precioTotal = 0; // variable inicializada a 0

            //  recuperar todas las disponibilidades del rango por puesto de trabajo
            var disponibilidadesAProcesar = new List<LineasPostReservaDTO>();

            if (reservaDTO.Lineas != null && reservaDTO.Lineas.Any())
            {
                // agrupar lineas del payload por idpuestotrabajo
                var gruposPorPuestoTrabajo = reservaDTO.Lineas
                    .GroupBy(l => l.IdPuestoTrabajo);

                foreach (var grupo in gruposPorPuestoTrabajo)
                {
                    var idPuestoTrabajo = grupo.Key;
                    var lineasDelGrupo = grupo.OrderBy(l => l.IdDisponibilidad).ToList();

                    if (lineasDelGrupo.Count >= 2)
                    {
                        // tomar primera y ultima disponibilidad del grupo para interar su rango
                        var idDisponibilidadInicio = lineasDelGrupo.First().IdDisponibilidad;
                        var idDisponibilidadFin = lineasDelGrupo.Last().IdDisponibilidad;

                        // obtener disponibilidades dentro del rango para el puesto de trabajo especifico
                        var disponibilidadesEnRango = await _context.Disponibilidades
                            .Where(d => d.IdPuestoTrabajo == idPuestoTrabajo &&
                                        d.IdDisponibilidad >= idDisponibilidadInicio &&
                                        d.IdDisponibilidad <= idDisponibilidadFin)
                            .ToListAsync();

                        // convertir disponibilidades encontradas a lineapostdto para procesar
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
                        // si solo hay una linea para este puesto de trabajo procesarla directamente
                        disponibilidadesAProcesar.Add(lineasDelGrupo.Single());
                    }
                    // si el grupo esta vacio no se hace nada
                }
            }

            // el bucle foreach ahora itera sobre disponibilidadesaprocesar que contendra todas las disponibilidades de todos los rangos
            foreach (var lineaDTO in disponibilidadesAProcesar)
            {
                // obtener disponibilidad directamente por su iddisponibilidad
                // incluir puestotrabajo y relaciones necesarias para calculo de precio
                var disponibilidad = await _context.Disponibilidades
                    .Include(d => d.PuestoTrabajo)
                        .ThenInclude(p => p.Sala)
                            .ThenInclude(s => s.TipoSala)
                                .ThenInclude(ts => ts.TipoPuestoTrabajo)
                    .FirstOrDefaultAsync(d => d.IdDisponibilidad == lineaDTO.IdDisponibilidad);

                if (disponibilidad == null)
                {
                    throw new ArgumentException($"No existe la disponibilidad con Id: {lineaDTO.IdDisponibilidad}");
                }

                // comprobar si esta disponible el campo estado de disponibilidad esta en true
                if (!disponibilidad.Estado)
                {
                    throw new InvalidOperationException($"El puesto de trabajo asociado a la disponibilidad {lineaDTO.IdDisponibilidad} no está disponible.");
                }

                // obtener puesto de trabajo de disponibilidad
                var puestoTrabajo = disponibilidad.PuestoTrabajo;

                if (puestoTrabajo == null)
                {
                    throw new ArgumentException($"La disponibilidad con Id: {lineaDTO.IdDisponibilidad} no tiene un puesto de trabajo asociado válido.");
                }

                // obtener precio base todavia no aplica extra de caracteristica para esa linea segun tipo de puesto
                decimal precioLinea = 0;
                if (puestoTrabajo.Sala?.TipoSala?.TipoPuestoTrabajo != null)
                {
                    precioLinea = puestoTrabajo.Sala.TipoSala.TipoPuestoTrabajo.Precio;
                }
                else
                {
                    throw new InvalidOperationException($"No se pudo determinar el precio base para el puesto de trabajo {puestoTrabajo.IdPuestoTrabajo}. Faltan datos de Sala/TipoSala/TipoPuestoTrabajo.");
                }

                // acceder a caracteristicas de sala ligadas a esa sala por id
                var caracteristicasSala = await _context.SalaConCaracteristicas
                    .Include(sc => sc.Caracteristica)
                    .Where(sc => sc.IdSala == puestoTrabajo.IdSala)
                    .ToListAsync();

                foreach (var caracteristica in caracteristicasSala)
                {
                    // aplicar precio aniadido por caracteristica
                    precioLinea += precioLinea * (caracteristica.Caracteristica.PrecioAniadido / 100m);
                }

                // creacion de linea dentro del foreach de lineas
                var linea = new Lineas
                {
                    IdReserva = reserva.IdReserva,
                    IdPuestoTrabajo = puestoTrabajo.IdPuestoTrabajo,
                    Precio = precioLinea
                };

                _context.Lineas.Add(linea);

                // poner disponibilidad en false despues de insertar linea
                disponibilidad.Estado = false;

                // sumar al precio total
                precioTotal += precioLinea;
            }

            // actualizar precio total de reserva una vez procesadas todas las lineas
            reserva.PrecioTotal = precioTotal;
            await _context.SaveChangesAsync();

            // confirmar la transacción si todo salió bien
            await transaction.CommitAsync();

            return reserva;
        }
        catch
        {
            // deshacer la transacción en caso de error
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

public async Task<List<GetReservasClienteDTO>> GetReservasUsuarioAsync(int idUsuario)
{
    // en vez de traer cada entidad por separado se proyecta todo en una sola consulta
    // seleccionamos las reservas que pertenecen al usuario usando el idUsuario
    // se ordenan por fecha descendente para que las mas recientes salgan primero
    var reservasConInfoBasica = await _context.Reservas
        .Where(r => r.IdUsuario == idUsuario)
        .OrderByDescending(r => r.Fecha)
        .Select(r => new
        {
            Reserva = r, // aqui se trae toda la entidad reserva completa
            LineasInfo = r.Lineas
                .Select(l => new // se proyecta solo la informacion necesaria de cada linea
                {
                    l.IdPuestoTrabajo,
                    NombreSala = l.PuestoTrabajo.Sala.Nombre,
                    UrlImagenSala = l.PuestoTrabajo.Sala.URL_Imagen,
                    CiudadSede = l.PuestoTrabajo.Sala.Sede.Ciudad,
                    DireccionSede = l.PuestoTrabajo.Sala.Sede.Direccion,
                    TieneInfoCompleta = l.PuestoTrabajo != null && l.PuestoTrabajo.Sala != null && l.PuestoTrabajo.Sala.Sede != null
                })
                .ToList()
        })
        .ToListAsync(); // al ejecutar esta consulta con tolistasync ya se trae todo en una sola llamada a base de datos
                       // muchisimo mas optimo ya que no se hacen llamadas separadas para cada propiedad relacionada como sala o sede
                       // todo se obtiene en la misma consulta gracias al select con proyeccion

    if (!reservasConInfoBasica.Any())
    {
        return new List<GetReservasClienteDTO>(); // si no hay reservas se retorna lista vacia
    }

    // obtener todos los ids de puestos de trabajo de todas las reservas
    var todosLosPuestoTrabajoIds = reservasConInfoBasica
        .SelectMany(rci => rci.LineasInfo.Select(li => li.IdPuestoTrabajo))
        .Distinct()
        .ToList();

    // se hace un solo llamado a la base de datos para traer todas las disponibilidades que correspondan
    List<Disponibilidad> todasLasDisponibilidadesRelevantes = new List<Disponibilidad>();
    if (todosLosPuestoTrabajoIds.Any())
    {
        todasLasDisponibilidadesRelevantes = await _context.Disponibilidades
            .Where(d => todosLosPuestoTrabajoIds.Contains(d.IdPuestoTrabajo) &&
                          d.Estado == false) // solo se traen las que estan reservadas
            .Include(d => d.TramoHorario) // se incluye el tramo horario en la misma consulta
            .ToListAsync(); // se ejecuta la consulta en una sola llamada
    }

    // se agrupan las disponibilidades por id de puesto de trabajo, para evitar consultas extra
    var disponibilidadesAgrupadas = todasLasDisponibilidadesRelevantes
        .GroupBy(d => d.IdPuestoTrabajo)
        .ToDictionary(g => g.Key, g => g.ToList());

    var resultado = new List<GetReservasClienteDTO>();

    // aqui ya no hay llamadas a la base de datos porque todo se obtuvo antes
    foreach (var reservaInfo in reservasConInfoBasica)
    {
        var reserva = reservaInfo.Reserva;
        var lineasInfo = reservaInfo.LineasInfo;

        var puestoTrabajoIdsDeEstaReserva = lineasInfo
            .Select(li => li.IdPuestoTrabajo)
            .Distinct()
            .ToList();

        var disponibilidadesAsociadas = new List<Disponibilidad>();
        foreach (var idPuesto in puestoTrabajoIdsDeEstaReserva)
        {
            if (disponibilidadesAgrupadas.TryGetValue(idPuesto, out var disponibilidadesDelPuesto))
            {
                disponibilidadesAsociadas.AddRange(
                    disponibilidadesDelPuesto.Where(d => d.Fecha.Date >= reserva.Fecha.Date)
                );
            }
        }

        // ordenar por fecha y hora
        disponibilidadesAsociadas = disponibilidadesAsociadas
                                    .OrderBy(d => d.Fecha)
                                    .ThenBy(d => d.TramoHorario.HoraInicio)
                                    .ToList();

        int cantidadHorasReservadas = disponibilidadesAsociadas.Count;

        string rangoHorario = "Horario no disponible";
        if (disponibilidadesAsociadas.Any())
        {
            DateTime minFecha = disponibilidadesAsociadas.Min(d => d.Fecha);
            DateTime maxFecha = disponibilidadesAsociadas.Max(d => d.Fecha);

            TimeSpan minHoraInicio = disponibilidadesAsociadas
                .Where(d => d.Fecha.Date == minFecha.Date)
                .Min(d => d.TramoHorario.HoraInicio);

            TimeSpan maxHoraFin = disponibilidadesAsociadas
                .Where(d => d.Fecha.Date == maxFecha.Date)
                .Max(d => d.TramoHorario.HoraFin);

            if (minFecha.Date == maxFecha.Date)
            {
                rangoHorario = $"{minFecha:dd/MM/yyyy} {minHoraInicio:hh\\:mm} - {maxHoraFin:hh\\:mm}";
            }
            else
            {
                rangoHorario = $"{minFecha:dd/MM/yyyy} {minHoraInicio:hh\\:mm} - {maxFecha:dd/MM/yyyy} {maxHoraFin:hh\\:mm}";
            }
        }
        else if (lineasInfo.Any())
        {
            rangoHorario = $"{reserva.Fecha:dd/MM/yyyy} (Horario no especificado)";
        }

        var primeraLineaValidaInfo = lineasInfo.FirstOrDefault(li => li.TieneInfoCompleta);

        resultado.Add(new GetReservasClienteDTO
        {
            IdReserva = reserva.IdReserva,
            PrecioTotal = reserva.PrecioTotal,
            CantidadHorasReservadas = cantidadHorasReservadas,
            NombreSalaPrincipal = primeraLineaValidaInfo?.NombreSala,
            ImagenSalaPrincipal = primeraLineaValidaInfo?.UrlImagenSala,
            CiudadSedePrincipal = primeraLineaValidaInfo?.CiudadSede,
            DireccionSedePrincipal = primeraLineaValidaInfo?.DireccionSede,
            RangoHorarioReserva = rangoHorario
        });
    }

    return resultado;
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