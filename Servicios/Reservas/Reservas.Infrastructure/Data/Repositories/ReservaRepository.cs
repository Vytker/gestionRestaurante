
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
        //lectura
        public IQueryable<Reserva> ObtenerTodas() //para superadmin sin filtro
       => _context.Reservas;
        public IQueryable<Reserva> ObtenerTodas(Guid restauranteId)
        {
            return _context.Reservas.Where(r => r.RestauranteId == restauranteId); // Cambiar el tipo de parámetro de int a Guid
        }


        public Reserva? ObtenerPorId(Guid id, Guid restauranteId) // Cambiar el tipo de parámetro de int a Guid
        {
            return _context.Reservas.FirstOrDefault(r => r.Id == id && r.RestauranteId == restauranteId); // Esto ahora compara correctamente dos valores de tipo Guid
        }

        public Reserva? ObtenerPorId(Guid id)
        {
            return _context.Reservas.FirstOrDefault(r => r.Id == id); // Esto ahora compara correctamente dos valores de tipo Guid
        }

        public bool ExistePorCode(string code, Guid restauranteId)
=> _context.Reservas.Any(r => r.Codigo == code && r.RestauranteId == restauranteId);

        public Reserva? ObtenerPorCode(string code, Guid restauranteId)
            => _context.Reservas.FirstOrDefault(r => r.Codigo == code && r.RestauranteId == restauranteId);

        public async Task<Turno?> ObtenerTurnoConReservasPorIdAsync(int turnoId, Guid restauranteId)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .FirstOrDefaultAsync(t => t.Id == turnoId && t.RestauranteId == restauranteId);
        }

        //Escritura
        public void Crear(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
        }
        public void Actualizar(Reserva reserva)
        {
            _context.Reservas.Update(reserva);  
        }
        public void Eliminar(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
        }
        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reserva>> ObtenerPorRangoAsync(Guid restauranteId, DateTime desde, DateTime hasta)
        {
            return await _context.Reservas
                .Where(r => r.RestauranteId == restauranteId && r.FechaReserva >= desde && r.FechaReserva <= hasta)
                .ToListAsync();
        }
    }
}
