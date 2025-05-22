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
            // Crear reserva
            var reserva = new Reservas
            {
                IdUsuario = reservaDTO.IdUsuario,
                Descripcion = reservaDTO.Descripcion,
                Fecha = reservaDTO.FechaReserva,
                PrecioTotal = 0 // Inicializado a 0, se actualizará conforme se añadan las líneas
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // Crear las líneas de esa reserva
            decimal precioTotal = 0; // Variable inicializada a 0

            foreach (var lineaDTO in reservaDTO.Lineas)
            {
                // Obtener la disponibilidad directamente por su IdDisponibilidad
                // Incluimos PuestoTrabajo y sus relaciones necesarias para el cálculo del precio
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

                // Comprobar si está disponible (el campo Estado de la tabla Disponibilidad está marcado como true)
                if (!disponibilidad.Estado)
                {
                    throw new InvalidOperationException($"El puesto de trabajo asociado a la disponibilidad {lineaDTO.IdDisponibilidad} no está disponible.");
                }

                // Obtener el puesto de trabajo de la disponibilidad
                var puestoTrabajo = disponibilidad.PuestoTrabajo;

                if (puestoTrabajo == null)
                {
                    // Esto no debería ocurrir si la FK está bien, pero es una buena práctica de seguridad
                    throw new ArgumentException($"La disponibilidad con Id: {lineaDTO.IdDisponibilidad} no tiene un puesto de trabajo asociado válido.");
                }

                // Obtener el precio base (todavía no está aplicado el extra de la característica) de esa línea por su tipo de puesto
                // Asegurarse de que todas las propiedades anidadas no sean nulas antes de acceder a ellas
                decimal precioLinea = 0;
                if (puestoTrabajo.Sala?.TipoSala?.TipoPuestoTrabajo != null)
                {
                    precioLinea = puestoTrabajo.Sala.TipoSala.TipoPuestoTrabajo.Precio;
                }
                else
                {
                    throw new InvalidOperationException($"No se pudo determinar el precio base para el puesto de trabajo {puestoTrabajo.IdPuestoTrabajo}. Faltan datos de Sala/TipoSala/TipoPuestoTrabajo.");
                }

                // Acceder a las características de la sala ligadas a esa sala con su ID
                var caracteristicasSala = await _context.SalaConCaracteristicas
                    .Include(sc => sc.Caracteristica)
                    .Where(sc => sc.IdSala == puestoTrabajo.IdSala)
                    .ToListAsync();

                foreach (var caracteristica in caracteristicasSala) // Un bucle, si tiene varias características, las recorre y va aplicando el precio añadido
                {
                    // Aplicar el precio añadido por característica
                    // El precio añadido se asume como un porcentaje (ej. 10 para 10%)
                    precioLinea += precioLinea * (caracteristica.Caracteristica.PrecioAniadido / 100m);
                    /*
                    El precio de la línea sería el precio de esa línea + su respectivo % de añadido, ejemplo: si vale 50€ y el precio añadido es 10%, será 50 + 5 = 55€.
                    La 'm' del 100 hace que lo trate como decimal para el tipo de dato que estamos usando.
                    */
                }

                // Creación de la línea, todo esto dentro del foreach de líneas
                var linea = new Lineas
                {
                    IdReserva = reserva.IdReserva,
                    IdPuestoTrabajo = puestoTrabajo.IdPuestoTrabajo, // Usamos el IdPuestoTrabajo de la disponibilidad
                    Precio = precioLinea
                };

                _context.Lineas.Add(linea);

                // Se pone la disponibilidad como no disponible (false) una vez ya se ha hecho el INSERT de la línea
                disponibilidad.Estado = false;

                // Sumar al precio total
                precioTotal += precioLinea;
            }

            // Actualizar el precio total de la reserva una vez recorridas todas las líneas
            reserva.PrecioTotal = precioTotal;
            await _context.SaveChangesAsync(); // Ahora que ya está el precio total final, habiendo pasado todas las líneas, se guarda la información

            return reserva;
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
            // Buscar la reserva por su ID
            var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva != null)// si existe, procede a eliminar
            {
                // primero elimina las linea asociadas para evitar que no se pueda por la FK
                var lineas = await _context.Lineas
                    .Where(l => l.IdReserva == id)
                    .ToListAsync();

                _context.Lineas.RemoveRange(lineas);

                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();
            }
        }


        // reestructurado el endpoint para recibir solo la info que se necesita y hacerlo mucho mas rapido
        public async Task<List<GetReservasClienteDTO>> GetReservasUsuarioAsync(int idUsuario)
        {
            // select inicial de reservas con lineas, puestoTrabajo, sala y sede, todas las tablas que se van a necesitar para ver la data
            var reservasConLineasYDetalles = await _context.Reservas
                .Where(r => r.IdUsuario == idUsuario)
                .OrderByDescending(r => r.Fecha) // descendente, primero las ultimas
                .Select(r => new
                {
                    Reserva = r,
                    // se juntan las columnas necesarias
                    LineasDetalladas = r.Lineas
                                        .Select(l => new
                                        {
                                            l.IdPuestoTrabajo, //ID del puesto para buscar disponibilidades
                                            PuestoTrabajo = l.PuestoTrabajo,
                                            Sala = l.PuestoTrabajo.Sala,
                                            Sede = l.PuestoTrabajo.Sala.Sede
                                        })
                                        .ToList() // se devuelve en una lista a usar a continuacion
                })
                .ToListAsync(); // Ejecuta la primera parte de la consulta a la base de datos

            var resultado = new List<GetReservasClienteDTO>();

            foreach (var reservaConLineas in reservasConLineasYDetalles) // recorrer sobre cada reserva conjuntada con el resto de campos 
            {
                var reserva = reservaConLineas.Reserva;
                var lineasDetalladas = reservaConLineas.LineasDetalladas;

                // obtener todos los IdPuestoTrabajo de las lineas de la reserva (el distintct evita los duplicados)
                var puestoTrabajoIds = lineasDetalladas.Select(ld => ld.IdPuestoTrabajo).Distinct().ToList();

                // obtener todas las disponibilidades para estos puestos de trabajo cuyo Estado = false y cuya fecha sea igual o posterior a la fecha de la reserva
                var disponibilidadesAsociadas = await _context.Disponibilidades
                    .Where(d => puestoTrabajoIds.Contains(d.IdPuestoTrabajo) &&
                                d.Estado == false && // Solo disponibilidades que ya están reservadas
                                d.Fecha.Date >= reserva.Fecha.Date) // Y que su fecha sea igual o posterior a la fecha de la reserva
                    .Include(d => d.TramoHorario) // Incluimos TramoHorario para poder poner tramo de inicio y fin
                    .OrderBy(d => d.Fecha)
                    .ThenBy(d => d.TramoHorario.HoraInicio) // Luego por hora de inicio
                    .ToListAsync();

                // se cuentan todas las disponibilidades ya que cada disponibilidad es una hora
                int cantidadHorasReservadas = disponibilidadesAsociadas.Count;

                // Determinar el rango horario consolidado para toda la reserva
                string rangoHorario = "Horario no disponible";
                if (disponibilidadesAsociadas.Any())
                {
                    // Encontrar la fecha de inicio y la fecha de fin de todas las disponibilidades asociadas a la reserva
                    DateTime minFecha = disponibilidadesAsociadas.Min(d => d.Fecha);
                    DateTime maxFecha = disponibilidadesAsociadas.Max(d => d.Fecha);

                    // Encontrar la hora de inicio
                    TimeSpan minHoraInicio = disponibilidadesAsociadas
                                                .Where(d => d.Fecha == minFecha)
                                                .Min(d => d.TramoHorario.HoraInicio);

                    // Encontrar la hora de fin
                    TimeSpan maxHoraFin = disponibilidadesAsociadas
                                                .Where(d => d.Fecha == maxFecha)
                                                .Max(d => d.TramoHorario.HoraFin);

                    if (minFecha.Date == maxFecha.Date)
                    {
                        // output formateado para 1 mismo dia
                        rangoHorario = $"{minFecha:dd/MM/yyyy} {minHoraInicio:hh\\:mm} - {maxHoraFin:hh\\:mm}";
                    }
                    else
                    {
                        // Formato para reservas de varios días
                        rangoHorario = $"{minFecha:dd/MM/yyyy} {minHoraInicio:hh\\:mm} - {maxFecha:dd/MM/yyyy} {maxHoraFin:hh\\:mm}";
                    }
                }
                else if (lineasDetalladas.Any())
                {
                    // output formateado para varios dias
                    rangoHorario = $"{reserva.Fecha:dd/MM/yyyy} (Horario no especificado)";
                }

                // obtener la información de la sala/sede de la primera línea valida (será la misma en todas las líneas)
                var primeraLineaValida = lineasDetalladas.FirstOrDefault(ld => ld.Sala != null && ld.Sede != null);

                resultado.Add(new GetReservasClienteDTO
                {
                    IdReserva = reserva.IdReserva,
                    PrecioTotal = reserva.PrecioTotal,
                    CantidadHorasReservadas = cantidadHorasReservadas,

                    NombreSalaPrincipal = primeraLineaValida?.Sala?.Nombre,
                    ImagenSalaPrincipal = primeraLineaValida?.Sala?.URL_Imagen,
                    CiudadSedePrincipal = primeraLineaValida?.Sede?.Ciudad,
                    DireccionSedePrincipal = primeraLineaValida?.Sede?.Direccion,
                    RangoHorarioReserva = rangoHorario
                });
            }

            return resultado;
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