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
    }
}
