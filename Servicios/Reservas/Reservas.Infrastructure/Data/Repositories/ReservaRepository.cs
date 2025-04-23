
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

        public void CrearReserva(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            _context.SaveChanges();
        }
        public void Crear(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            _context.SaveChanges();
        }

        public async Task<(bool Disponible, string? Error, string Code)> CrearReservaAsync(Application.Dtos.ReservaCreateDto dto)
        {
            var turno = await _context.Turnos
                .Include(t => t.Reservas)
                .FirstOrDefaultAsync(t => t.Id == dto.TurnoId);
            if (turno == null)
            {
                return (false, "Turno no encontrado.", null);
            }
            var comensalesYaReservados = turno.Reservas
                .Where(r => r.FechaReserva.Date == dto.FechaReserva.Date)
                .Sum(r => r.NumeroComensales);
            if (comensalesYaReservados + dto.NumeroComensales > turno.Capacidad)
            {
                return (false, "No hay suficiente capacidad en el turno seleccionado.", null);
            }
            var reserva = new Reserva
            {
                NombreCliente = dto.NombreCliente,
                FechaReserva = dto.FechaReserva,
                NumeroComensales = dto.NumeroComensales,
                Notas = dto.Notas,
                TurnoId = dto.TurnoId
            };
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            return (true, null, reserva.Codigo);
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
        public bool ExistePorCodigo(string codigo)
        {
            return _context.Reservas.Any(r => r.Codigo == codigo);
        }
        public Reserva? ObtenerPorCodigo(string codigo)
        {
            return _context.Reservas.FirstOrDefault(r => r.Codigo == codigo);
        }
        public void Eliminar(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
            _context.SaveChanges();
        }
        public void Eliminar(Guid id)
        {
            var reserva = ObtenerPorId(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
                _context.SaveChanges();
            }
        }
        public bool ExistePorCode(string code)
       => _context.Reservas.Any(r => r.Codigo== code);

        public Reserva? ObtenerPorCode(string code)
            => _context.Reservas.FirstOrDefault(r => r.Codigo== code);


        public void EliminarReserva(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
            _context.SaveChanges();
        }

    }
}
