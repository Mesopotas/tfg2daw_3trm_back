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
        
        public DbSet<ZonasTrabajo> ZonasTrabajo{ get; set; }
        public DbSet<Reservas> Reservas { get; set; }
        public DbSet<Lineas> Lineas { get; set; }

        public DbSet<SalaConCaracteristicas> SalaConCaracteristicas { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // si esto no se define pondrá Invalid column name 'TipoPuestoTrabajoIdTipoPuestoTrabajo'
    modelBuilder.Entity<TiposSalas>()
        .HasOne(tipospuesto => tipospuesto.TipoPuestoTrabajo) // HasOne hace 1:-
        .WithMany()  // WithMany seria -:N , es decir , la relacion 1:N
        .HasForeignKey(tipospuesto => tipospuesto.IdTipoPuestoTrabajo); // FK presente tipossalas a tipospuestotrabajo
}

}


}