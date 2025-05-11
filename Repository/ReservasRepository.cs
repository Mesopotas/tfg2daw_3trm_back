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



        
 public async Task<List<GetReservasClienteDTO>> GetReservasUsuarioAsync(int idUsuario)
    {
        // obtener todas las reservas
        var reservas = await _context.Reservas
            .Where(r => r.IdUsuario == idUsuario)
            .Include(r => r.Usuario)
            .Select(r => new GetReservasClienteDTO
            {
                IdReserva = r.IdReserva,
                FechaReserva = r.Fecha,
                DescripcionReserva = r.Descripcion,
                PrecioTotal = r.PrecioTotal,
                UsuarioNombre = r.Usuario.Nombre,
                UsuarioApellidos = r.Usuario.Apellidos,
                UsuarioEmail = r.Usuario.Email,
                Lineas = new List<GetReservasClienteLineaDTO>()
            })
            .ToListAsync();

        // obtener todas las lineas de cada una de esas reservas
        foreach (var reserva in reservas)
        {
            // recorrer todos los campos de los que se va a extraer informacion
            var lineas = await (
                from l in _context.Lineas
                join pt in _context.PuestosTrabajo on l.IdPuestoTrabajo equals pt.IdPuestoTrabajo // asegura que compartan el mismo ID
                join zt in _context.ZonasTrabajo on pt.IdZonaTrabajo equals zt.IdZonaTrabajo
                join s in _context.Salas on pt.IdSala equals s.IdSala
                join tps in _context.TiposSalas on s.IdTipoSala equals tps.IdTipoSala
                join tpt in _context.TiposPuestosTrabajo on tps.IdTipoPuestoTrabajo equals tpt.IdTipoPuestoTrabajo
                join sd in _context.Sedes on s.IdSede equals sd.IdSede
                join d in _context.Disponibilidades on pt.IdPuestoTrabajo equals d.IdPuestoTrabajo into dispGroup
                from d in dispGroup.DefaultIfEmpty()// left join, si no hay sera null
                join th in _context.TramosHorarios on d.IdTramoHorario equals th.IdTramoHorario into thGroup
                from th in thGroup.DefaultIfEmpty() // left join, si no hay sera null
                where l.IdReserva == reserva.IdReserva
                select new
                {
                    // Linea
                    l.IdLinea,
                    l.Precio,
                    pt.NumeroAsiento,
                    pt.CodigoMesa,
                    ImagenPuesto = pt.URL_Imagen,
                    s.IdSala,
                    
                    // Zona
                    IdZonaTrabajo = zt.IdZonaTrabajo,
                    DescripcionZona = zt.Descripcion,
                    
                    // Sala
                    NombreSala = s.Nombre,
                    ImagenSala = s.URL_Imagen,
                    TipoSala = tps.Nombre,
                    tps.EsPrivada,
                    TipoPuestoTrabajo = tpt.Nombre,
                    DescripcionPuesto = tpt.Descripcion,
                    
                    // Sede
                    IdSede = sd.IdSede,
                    sd.Pais,
                    sd.Ciudad,
                    sd.Direccion,
                    sd.CodigoPostal,
                    sd.Planta,
                    
                    // Horario
                    /*si cualquier valor de la fecha no es null, le da su valor, sino null,
                    si no se pone la comprobacion ? : da error CS8072*/
                    FechaDisponibilidad = d != null ? (DateTime?)d.Fecha : null,
                  HoraInicio = th != null ? (TimeSpan?)th.HoraInicio : null,
                 HoraFin = th != null ? (TimeSpan?)th.HoraFin : null,

                }
            ).ToListAsync();

        
            var idsSalas = lineas.Select(l => l.IdSala).Distinct().ToList(); // select para los ids de las salas en las lineas,distinct evita duplicados, si todas son id = 1 solo habra 1
// caracteristicas de cada sala en cada linea
var caracteristicasPorSala = (
    from salaconcaracteristicas in _context.SalaConCaracteristicas
    join cs in _context.CaracteristicasSala on salaconcaracteristicas.IdCaracteristica equals cs.IdCaracteristica // join a CaracteristicasSala
    where idsSalas.Contains(salaconcaracteristicas.IdSala) // filtro al id de la sala
    select new // select de los 2 datos que se quiere sacar en la respuesta
    {
        salaconcaracteristicas.IdSala,
        cs.Nombre
    }
).ToList();

foreach (var linea in lineas) // una reserva puede tener N lineas, las recorre
{
    // caracteristicas con la info de su sala
    var caracteristicas = string.Join(
        ", ", // junta todos los nombres que haya con , en un solo string, de la forma que sera tipo proyector, aire acondicionado, etc
        caracteristicasPorSala
            .Where(c => c.IdSala == linea.IdSala) // caracteristicas de solo la sala elegida con su id
            .Select(c => c.Nombre)
    );

    // linea con la info de su tabla
    reserva.Lineas.Add(new GetReservasClienteLineaDTO
    {
        IdLinea = linea.IdLinea,
        PrecioLinea = linea.Precio,
        NumeroAsiento = linea.NumeroAsiento,
        CodigoMesa = linea.CodigoMesa,
        ImagenPuesto = linea.ImagenPuesto,
        
        Zona = new GetReservasClienteZonaDTO
        {
            IdZonaTrabajo = linea.IdZonaTrabajo,
            Descripcion = linea.DescripcionZona
        },
        
        Sala = new GetReservasClienteSalaDTO
        {
            IdSala = linea.IdSala,
            Nombre = linea.NombreSala,
            ImagenSala = linea.ImagenSala,
            TipoSala = linea.TipoSala,
            EsPrivada = linea.EsPrivada,
            TipoPuestoTrabajo = linea.TipoPuestoTrabajo,
            DescripcionPuesto = linea.DescripcionPuesto,
            CaracteristicasSala = caracteristicas,
            Sede = new GetReservasClienteSedeDTO
            {
                IdSede = linea.IdSede,
                Pais = linea.Pais,
                Ciudad = linea.Ciudad,
                Direccion = linea.Direccion,
                CodigoPostal = linea.CodigoPostal,
                Planta = linea.Planta,
            }
        },
        
        Horario = new GetReservasClienteHorarioDTO
        {
            FechaDisponibilidad = linea.FechaDisponibilidad,
            HoraInicio = linea.HoraInicio,
            HoraFin = linea.HoraFin
        }
    });
}

        }

        // devolver reserva con lineas y toda la info necesaria
        return reservas.OrderByDescending(r => r.FechaReserva).ToList(); //OrderByDescending hace q salgan primero las reservas mas nuevas, devolviendo un array con las reservas del usuario
    }

}

}