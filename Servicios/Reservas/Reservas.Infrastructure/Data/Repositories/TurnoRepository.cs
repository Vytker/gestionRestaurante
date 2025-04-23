using Microsoft.EntityFrameworkCore;
using Reservas.Application.Interfaces;
using Reservas.Domain.Entities;

namespace Reservas.Infrastructure.Data.Repositories
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly ReservasDbContext _context;
        public TurnoRepository(ReservasDbContext context)
        {
            _context = context;
        }

        public void Crear(Turno turno)
        {
            _context.Turnos.Add(turno);
        }
        public void Actualizar(Turno turno)
        {
            _context.Turnos.Update(turno);
        }
        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTodosAsync()
        {
            return await _context.Turnos.Where(t => !t.Eliminado).ToListAsync();
        }
        public async Task<Turno?> ObtenerPorIdAsync(int id)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => !t.Eliminado) //ignora los turnos eliminados
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<Turno?> ObtenerTurnoConReservasPorIdAsync(int turnoId)
        {
            return await _context.Turnos.FirstOrDefaultAsync(t => t.Id == turnoId && !t.Eliminado);
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorFechaAsync(DateTime fecha)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.FechaReserva.Date == fecha.Date))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorClienteAsync(string nombreCliente)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.NombreCliente == nombreCliente))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorEstadoAsync(Reserva.EstadoReserva estado)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.Estado == estado))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorComensalesAsync(int numeroComensales)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.NumeroComensales == numeroComensales))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorNotasAsync(string notas)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.Notas.Contains(notas)))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.FechaReserva >= fechaInicio && r.FechaReserva <= fechaFin))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorRangoComensalesAsync(int minComensales, int maxComensales)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.NumeroComensales >= minComensales && r.NumeroComensales <= maxComensales))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorEstadoYFechaAsync(Reserva.EstadoReserva estado, DateTime fecha)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.Estado == estado && r.FechaReserva.Date == fecha.Date))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorEstadoYClienteAsync(Reserva.EstadoReserva estado, string nombreCliente)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.Estado == estado && r.NombreCliente == nombreCliente))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorEstadoYComensalesAsync(Reserva.EstadoReserva estado, int numeroComensales)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.Estado == estado && r.NumeroComensales == numeroComensales))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorEstadoYNotasAsync(Reserva.EstadoReserva estado, string notas)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.Estado == estado && r.Notas.Contains(notas)))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorEstadoYRangoFechasAsync(Reserva.EstadoReserva estado, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.Estado == estado && r.FechaReserva >= fechaInicio && r.FechaReserva <= fechaFin))
                .ToListAsync();
        }

        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorEstadoYRangoComensalesAsync(Reserva.EstadoReserva estado, int minComensales, int maxComensales)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.Estado == estado && r.NumeroComensales >= minComensales && r.NumeroComensales <= maxComensales))
                .ToListAsync();
        }
        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorFechaYComensalesAsync(DateTime fecha, int numeroComensales)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.FechaReserva.Date == fecha.Date && r.NumeroComensales == numeroComensales))
                .ToListAsync();
        }


        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorFechaYNotasAsync(DateTime fecha, string notas)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.FechaReserva.Date == fecha.Date && r.Notas.Contains(notas)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Turno>> ObtenerTurnosConReservasPorComensalesYNotasAsync(int numeroComensales, string notas)
        {
            return await _context.Turnos
                .Include(t => t.Reservas)
                .Where(t => t.Reservas.Any(r => r.NumeroComensales == numeroComensales && r.Notas.Contains(notas)))
                .ToListAsync();
        }

        public async Task<Turno?> ObtenerPorIdAsync(int? id)
        {
            if (id == null) return null;
            return await _context.Turnos
                .Include(t => t.Reservas)
                .FirstOrDefaultAsync(t => t.Id == id && !t.Eliminado);
        }

    }
}
