using Microsoft.EntityFrameworkCore;
using Models;

namespace CoWorking.Data
{
    public class CoworkingDBContext : DbContext
    {
        public CoworkingDBContext(DbContextOptions<CoworkingDBContext> options) : base(options) { }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Roles> Roles { get; set; }

    }
}
