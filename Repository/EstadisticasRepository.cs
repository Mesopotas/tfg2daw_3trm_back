using Microsoft.EntityFrameworkCore;
using CoWorking.Data;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public class EstadisticasRepository : IEstadisticasRepository
    {
        private readonly CoworkingDBContext _context;

        public EstadisticasRepository(CoworkingDBContext context)
        {
            _context = context;
        }
        public async Task<DashboardEstadisticasDTO> GetDashboardEstadisticasAsync()
        {
            // fecha y hora actual
            var ahora = DateTime.Now;

            // primer dia del mes actual
            var inicioMes = new DateTime(ahora.Year, ahora.Month, 1);

            // primer dia del año actual
            var inicioAnio = new DateTime(ahora.Year, 1, 1);


            // total de usuarios registrados
            var totalUsuarios = await _context.Usuarios.CountAsync();

            // total de reservas realizadas
            var totalReservas = await _context.Reservas.CountAsync();

            // total de reservas hechas en el mes actual
            var reservasMesActual = await _context.Reservas
                .Where(r => r.Fecha >= inicioMes)
                .CountAsync();

            // suma total de ingresos generados por reservas
            var ingresosTotales = await _context.Reservas
                .SumAsync(r => r.PrecioTotal);

            // suma de ingresos solo en el mes actual
            var ingresosMesActual = await _context.Reservas
                .Where(r => r.Fecha >= inicioMes)
                .SumAsync(r => r.PrecioTotal);

            // reservas agrupadas por sede
            // se hace un join entre reservas, lineas, puestos, salas y sedes para agrupar por ciudad
            var reservasPorSede = await _context.Reservas
                .Join(_context.Lineas, r => r.IdReserva, l => l.IdReserva, (r, l) => new { r, l })
                .Join(_context.PuestosTrabajo, rl => rl.l.IdPuestoTrabajo, pt => pt.IdPuestoTrabajo, (rl, pt) => new { rl.r, pt })
                .Join(_context.Salas, rpt => rpt.pt.IdSala, s => s.IdSala, (rpt, s) => new { rpt.r, s })
                .Join(_context.Sedes, rs => rs.s.IdSede, sede => sede.IdSede, (rs, sede) => new { rs.r, sede })
                .GroupBy(x => new { x.sede.Ciudad, x.sede.IdSede })
                .Select(g => new ReservasPorSedeDTO
                {
                    Ciudad = g.Key.Ciudad,
                    CantidadReservas = g.Count(),
                    Ingresos = g.Sum(x => x.r.PrecioTotal)
                })
                .OrderByDescending(x => x.CantidadReservas)
                .ToListAsync();

            // reservas por mes en los ultimos 6 meses
            // se calcula la fecha de hace 6 meses desde ahora
            var hace6Meses = ahora.AddMonths(-6);

            // se agrupan las reservas por año y mes y se cuentan
            var reservasPorMes = await _context.Reservas
                .Where(r => r.Fecha >= hace6Meses)
                .GroupBy(r => new { Año = r.Fecha.Year, Mes = r.Fecha.Month })
                .Select(g => new ReservasPorMesDTO
                {
                    Año = g.Key.Año,
                    Mes = g.Key.Mes,
                    CantidadReservas = g.Count(),
                    Ingresos = g.Sum(r => r.PrecioTotal)
                })
                .OrderBy(x => x.Año).ThenBy(x => x.Mes)
                .ToListAsync();

            // tipos de sala mas reservados
            // se hace join hasta llegar al tipo de sala y se agrupa por nombre
            var tiposSalaMasReservados = await _context.Reservas
                .Join(_context.Lineas, r => r.IdReserva, l => l.IdReserva, (r, l) => new { r, l })
                .Join(_context.PuestosTrabajo, rl => rl.l.IdPuestoTrabajo, pt => pt.IdPuestoTrabajo, (rl, pt) => new { rl.r, pt })
                .Join(_context.Salas, rpt => rpt.pt.IdSala, s => s.IdSala, (rpt, s) => new { rpt.r, s })
                .Join(_context.TiposSalas, rs => rs.s.IdTipoSala, ts => ts.IdTipoSala, (rs, ts) => new { rs.r, ts })
                .GroupBy(x => x.ts.Nombre)
                .Select(g => new TipoSalaReservasDTO
                {
                    TipoSala = g.Key,
                    CantidadReservas = g.Count(),
                    Ingresos = g.Sum(x => x.r.PrecioTotal)
                })
                .OrderByDescending(x => x.CantidadReservas)
                .ToListAsync();

            // ocupacion por dia de la semana (ultimos 30 dias)
            // se toman solo las disponibilidades ocupadas en los ultimos 30 dias
            var hace30Dias = ahora.AddDays(-30);

            var disponibilidades = await _context.Disponibilidades
                .Where(d => d.Fecha >= hace30Dias && d.Estado == false) // estado false indica ocupado
                .ToListAsync();

            // se agrupan las disponibilidades por dia de la semana y se cuentan las horas ocupadas
            var ocupacionPorDia = disponibilidades
                .GroupBy(d => (int)d.Fecha.DayOfWeek)
                .Select(g => new OcupacionDiaDTO
                {
                    DiaSemana = g.Key,
                    HorasOcupadas = g.Count()
                })
                .ToList();

            // top 5 usuarios mas activos
            // se agrupan reservas por usuario y se calculan las estadisticas
            var usuariosMasActivos = await _context.Reservas
                .GroupBy(r => new { r.IdUsuario, r.Usuario.Nombre, r.Usuario.Email })
                .Select(g => new UsuarioActivoDTO
                {
                    NombreUsuario = g.Key.Nombre,
                    Email = g.Key.Email,
                    CantidadReservas = g.Count(),
                    TotalGastado = g.Sum(r => r.PrecioTotal)
                })
                .OrderByDescending(x => x.CantidadReservas)
                .Take(5) // seleccionar solo los 5 primeros
                .ToListAsync();

            // output estadisticas    

            return new DashboardEstadisticasDTO
            {
                TotalUsuarios = totalUsuarios,
                TotalReservas = totalReservas,
                ReservasMesActual = reservasMesActual,
                IngresosTotales = ingresosTotales,
                IngresosMesActual = ingresosMesActual,

                ReservasPorSede = reservasPorSede,
                ReservasPorMes = reservasPorMes,
                TiposSalaMasReservados = tiposSalaMasReservados,
                OcupacionPorDia = ocupacionPorDia,
                UsuariosMasActivos = usuariosMasActivos
            };
        }


    }
}
