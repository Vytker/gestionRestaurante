using Microsoft.EntityFrameworkCore;

namespace Reservas.Infrastructure.Data
{
    public class ReservasDbContext : DbContext
    {
        public ReservasDbContext(DbContextOptions<ReservasDbContext> options) : base(options)
        {

        }
        public DbSet<Reservas.Domain.Entities.Reserva> Reservas { get; set; }
    }
}
