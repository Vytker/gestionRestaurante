
using Microsoft.EntityFrameworkCore;
using Reservas.Domain.Entities;
using Reservas.Infrastructure.Data;

namespace Reservas.Infrastructure.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly ReservasDbContext _context;

        public ReservaRepository(ReservasDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Reserva> ObtenerTodas()
        {
            return _context.Reservas.ToList();
        }

        public Reserva? ObtenerPorId(Guid id) // Cambiar el tipo de parámetro de int a Guid
        {
            return _context.Reservas.FirstOrDefault(r => r.Id == id); // Esto ahora compara correctamente dos valores de tipo Guid
        }

        public void Crear(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            _context.SaveChanges();
        }

        public void Actualizar(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            _context.SaveChanges();
        }

        public async Task<Turno?> ObtenerTurnoConReservasPorIdAsync(int turnoId)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .FirstOrDefaultAsync(t => t.Id == turnoId);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
