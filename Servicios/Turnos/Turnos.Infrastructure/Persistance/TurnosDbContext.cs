using EnvDTE;
using Microsoft.EntityFrameworkCore;
using Turnos.Domain.Entities;
using Turnos.Infrastructure.Configurations;

namespace Turnos.Infrastructure.Persistence
{
    public class TurnosDbContext : DbContext
    {
        // Ahora con genérico para que EF CLI lo reconozca
        public TurnosDbContext(DbContextOptions<TurnosDbContext> options)
            : base(options)
        { }

        public DbSet<Turno> Turnos { get; set; }  // añadido tipo genérico

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TurnoConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
