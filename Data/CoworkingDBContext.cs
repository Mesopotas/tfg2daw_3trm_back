using Microsoft.EntityFrameworkCore;
using Models;

namespace CoWorking.Data
{
    public class CoworkingDBContext : DbContext
    {
        public CoworkingDBContext(DbContextOptions<CoworkingDBContext> options) : base(options) { }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<CaracteristicasSala> CaracteristicasSala { get; set; }
        public DbSet<Sedes> Sedes { get; set; }
        public DbSet<Disponibilidad> Disponibilidades { get; set; }

         public DbSet<PuestosTrabajo> PuestosTrabajo { get; set; }
        public DbSet<TramosHorarios> TramosHorarios { get; set; }
        public DbSet<TiposPuestosTrabajo> TiposPuestosTrabajo { get; set; }
        public DbSet<TiposSalas> TiposSalas { get; set; }
        public DbSet<Salas> Salas{ get; set; }

    }
}
