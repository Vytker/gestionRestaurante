using Microsoft.EntityFrameworkCore;

namespace Reservas.Infrastructure.Data
{
    public class ReservasDbContext : DbContext
    {
        public ReservasDbContext(DbContextOptions<ReservasDbContext> options) : base(options)
        {

        }
        public DbSet<Domain.Entities.Reserva> Reservas { get; set; }
        public DbSet<Domain.Entities.Turno> Turnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuración de la entidad Reserva
            
           
            
            modelBuilder.Entity<Domain.Entities.Reserva>()
                .ToTable("Reservas")
                .HasKey(r => r.Id);
            // Configuración de la entidad Turno
            modelBuilder.Entity<Domain.Entities.Turno>()
                .ToTable("Turnos")
                .HasKey(t => t.Id);

            modelBuilder.Entity<Domain.Entities.Turno>()
                .HasMany(t => t.Reservas)
                .WithOne(r => r.Turno)
                .HasForeignKey(r => r.TurnoId)
                .OnDelete(DeleteBehavior.Cascade); // Configuración de la relación entre Turno y Reserva

            modelBuilder.Entity<Domain.Entities.Turno>()
                .HasIndex(t => new { t.RestauranteId, t.Nombre })
                .IsUnique()
                .HasFilter("[Eliminado] = 0"); // Índice único para RestauranteId y Nombre, considerando el filtro de eliminado

            modelBuilder.Entity<Domain.Entities.Reserva>()
                .HasIndex(r => new { r.RestauranteId, r.Codigo })
                .IsUnique();
                
        }
    }
}
