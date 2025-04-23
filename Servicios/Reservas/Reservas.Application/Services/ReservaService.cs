using System.IO;
using Reservas.Application.Dtos;
using Reservas.Application.Interfaces;
using Reservas.Domain.Entities;

namespace Reservas.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly INotificationService _notificationService;

        public ReservaService(
            IReservaRepository reservaRepository,
            INotificationService notificationService)
        {
            _reservaRepository = reservaRepository;
            _notificationService = notificationService;
        }

        public IEnumerable<Reserva> ObtenerTodasReservas()
            => _reservaRepository.ObtenerTodas();

        public async Task<(bool Disponible, string? Error, string Codigo)> CrearReservaAsync(ReservaCreateDto reservaDto)
        {
            var turno = await _reservaRepository.ObtenerTurnoConReservasPorIdAsync(reservaDto.TurnoId);
            if (turno == null)
                return (false, "Turno no encontrado.", string.Empty);

            var reservados = turno.Reservas
                .Where(r => r.FechaReserva.Date == reservaDto.FechaReserva.Date)
                .Sum(r => r.NumeroComensales);

            if (reservados + reservaDto.NumeroComensales > turno.Capacidad)
                return (false, "No hay suficiente capacidad.", string.Empty);

            // Generar Código único
            string codigo;
            do
            {
                codigo = Path.GetRandomFileName()
                             .Replace(".", "")
                             .Substring(0, 8)
                             .ToUpper();
            }
            while (_reservaRepository.ExistePorCode(codigo));

            // Crear reserva
            var reserva = new Reserva
            {
                NombreCliente = reservaDto.NombreCliente,
                Email = reservaDto.Email,
                FechaReserva = reservaDto.FechaReserva,
                NumeroComensales = reservaDto.NumeroComensales,
                Notas = reservaDto.Notas,
                TurnoId = reservaDto.TurnoId,
                Codigo = codigo    // coincide con tu entidad y DTO
            };
            _reservaRepository.Crear(reserva);
            await _reservaRepository.GuardarCambiosAsync();

            // Notificar
            await _notificationService.NotifyReservationCreatedAsync(reserva);

            return (true, null, codigo);
        }

        public void ActualizarEstadoReserva(Guid id, Reserva.EstadoReserva nuevoEstado)
        {
            var reserva = _reservaRepository.ObtenerPorId(id)
                ?? throw new KeyNotFoundException("Reserva no encontrada.");
            reserva.Estado = nuevoEstado;
            _reservaRepository.Actualizar(reserva);
        }

        public async Task<ReservaDto?> ObtenerReservaPorCodeAsync(string code)
        {
            var r = _reservaRepository.ObtenerPorCode(code);
            if (r == null) return null;
            return new ReservaDto
            {
                Id = r.Id,
                NombreCliente = r.NombreCliente,
                Email= r.Email,
                FechaReserva = r.FechaReserva,
                NumeroComensales = r.NumeroComensales,
                Notas = r.Notas,
                Estado = r.Estado.ToString(),
                TurnoId = r.TurnoId,
                Codigo = r.Codigo
            };
        }

        public async Task<bool> ActualizarReservaPorCodeAsync(string code, ReservaUpdateDto dto)
        {
            var r = _reservaRepository.ObtenerPorCode(code);
            if (r == null) return false;
            r.FechaReserva = dto.FechaReserva;
            r.NumeroComensales = dto.NumeroComensales;
            _reservaRepository.Actualizar(r);
            await _reservaRepository.GuardarCambiosAsync();
            return true;
        }

        public async Task<bool> CancelarReservaPorCodeAsync(string code)
        {
            var r = _reservaRepository.ObtenerPorCode(code);
            if (r == null) return false;
            r.Estado = Reserva.EstadoReserva.Cancelada;
            _reservaRepository.Actualizar(r);
            await _reservaRepository.GuardarCambiosAsync();
            return true;
        }

        public bool ExistePorCode(string code)
        {
            throw new NotImplementedException();
        }
    }
}
