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
        public async Task<List<PuestosTrabajoDTO>> GetDisponiblesEnSedeAsync(int idSede, DateTime fechaInicio, DateTime fechaFin, TimeSpan horaInicio, TimeSpan horaFin)
        {
  
                        
            var filtro = _context.PuestosTrabajo // todos los puestos de trabajo como base
                .Join(_context.Salas, // iner join a salas
                    puesto => puesto.IdSala, // une cada puesto de trabajo con la sala que le corresponde
                    sala => sala.IdSala,
                    (puesto, sala) => new { puesto, sala }) // union de puesto y sala
                .Join(_context.Sedes,
                    puestoysala => puestoysala.sala.IdSede, // une lo del join de arriba (puestos y salas) con sedes
                    sede => sede.IdSede,
                    (puestoysala, sede) => new { puestoysala.puesto, puestoysala.sala, sede })
                .Join(_context.Disponibilidades,
                    pdisponibilidad => pdisponibilidad.puesto.IdPuestoTrabajo, // union de lo de arriba (puesto, sala, sede) con su disponibilidad 
                    disponibilidad => disponibilidad.IdPuestoTrabajo,
                    (pdisponibilidad, disponibilidad) => new { pdisponibilidad.puesto, pdisponibilidad.sala, pdisponibilidad.sede, disponibilidad })
                .Join(_context.TramosHorarios,
                    ptramohorario => ptramohorario.disponibilidad.IdTramoHorario, // une lo de arriba (puesto, sala, sede, disponibilidad) con su tramo horario
                    tramoHorario => tramoHorario.IdTramoHorario,
                    (ptramohorario, tramoHorario) => new
                    { // objeto final para el output con todas las uniones
                        PuestoEntidad = ptramohorario.puesto,
                        SalaEntidad = ptramohorario.sala,
                        SedeEntidad = ptramohorario.sede,
                        DisponibilidadEntidad = ptramohorario.disponibilidad,
                        TramoHorarioEntidad = tramoHorario
                    })
                .Where(puesto => puesto.SedeEntidad.IdSede == idSede // solo las q pertenezcan a la sede elegida
                             && puesto.PuestoEntidad.Disponible == true // si el puesto esta disponible (tal vez quitar)
                             && puesto.DisponibilidadEntidad.Estado == true // si no esta disponible no la saca (tal vez quitar este filtro)
                             && puesto.DisponibilidadEntidad.Fecha >= fechaInicio && puesto.DisponibilidadEntidad.Fecha <= fechaFin // sea mayor o igual que la fecha de comienzo y menor o igual que la fecha de fin
                             && puesto.TramoHorarioEntidad.HoraInicio >= horaInicio && puesto.TramoHorarioEntidad.HoraFin <= horaFin) // sea mayor o igual que la hora de comienzo y menor o igual que la hora de fin, si el tramo horario es 08:00-09:00, horafin deberá ser al menos 09:00
                .Select(puesto => new PuestosTrabajoDTO // dar los valores al dto q será el output
                {
                    IdPuestoTrabajo = puesto.PuestoEntidad.IdPuestoTrabajo,
                    NumeroAsiento = puesto.PuestoEntidad.NumeroAsiento,
                    CodigoMesa = puesto.PuestoEntidad.CodigoMesa,
                    URL_Imagen = puesto.PuestoEntidad.URL_Imagen,
                    Disponible = puesto.PuestoEntidad.Disponible,
                    Bloqueado = puesto.PuestoEntidad.Bloqueado,
                    IdZonaTrabajo = puesto.PuestoEntidad.IdZonaTrabajo,
                    IdSala = puesto.PuestoEntidad.IdSala
                });


            return await filtro.ToListAsync();
        }
    }
}