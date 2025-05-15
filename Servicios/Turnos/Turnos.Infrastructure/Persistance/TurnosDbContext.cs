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

        public DbSet<Slot> Slots { get; set; } = null!;
        public DbSet<Assignment> Assignments { get; set; } = null!;
        public DbSet<Turno> Turnos { get; set; } = null!; // si aún lo usas




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TurnoConfiguration());
            base.OnModelCreating(modelBuilder);
            // Turnos
            modelBuilder.Entity<Turno>(b => {
                b.OwnsOne(t => t.Horario, h => {
                    h.Property(x => x.Inicio).HasColumnName("HorarioInicio");
                    h.Property(x => x.Fin).HasColumnName("HorarioFin");
                });
            });

            // Slots
            modelBuilder.Entity<Slot>(b => {
                b.OwnsOne(s => s.Horario, h => {
                    h.Property(x => x.Inicio).HasColumnName("HorarioInicio");
                    h.Property(x => x.Fin).HasColumnName("HorarioFin");
                });
            });
        }


    }
}
