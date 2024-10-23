using AplicacionNPG.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AplicacionNPG.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> usuario { get; set; }
        public DbSet<Incidencia> incidencia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .Property(u => u.HoraRegistro)
                .HasDefaultValueSql("GETDATE()");
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().HasKey(i => i.Id);
            modelBuilder.Entity<Incidencia>().HasKey(i => i.Id);
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Incidencia>().ToTable("incidencia");
        }
    }
}
