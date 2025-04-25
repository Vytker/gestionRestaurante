
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

        public IEnumerable<Reserva> ObtenerTodasReservas(Guid restauranteId)
            => _reservaRepository.ObtenerTodas(restauranteId);

        //crear reserva
        public async Task<(bool Disponible, string? Error, string Codigo)> CrearReservaAsync(Guid restauranteId, ReservaCreateDto reservaDto)
        {
            var turno = await _reservaRepository.ObtenerTurnoConReservasPorIdAsync(reservaDto.TurnoId, restauranteId);
           
            if (turno == null)
                return (false, "Turno no encontrado.", string.Empty);

            var reservados = turno.Reservas
                .Where(r => r.FechaReserva.Date == reservaDto.FechaReserva.Date)
                .Sum(r => r.NumeroComensales);

            var hora = reservaDto.FechaReserva.TimeOfDay;
            if(hora < turno.HoraInicio || hora >= turno.HoraFin)
            {
                return (false, "La hora de la reserva no está dentro del turno.", string.Empty);
            }

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
            while (_reservaRepository.ExistePorCode(codigo, restauranteId));

            // Crear reserva
            var reserva = new Reserva
            {
                RestauranteId = restauranteId,
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

        public void ActualizarEstadoReserva(Guid id, Guid restauranteId,Reserva.EstadoReserva nuevoEstado)
        {
            var reserva = _reservaRepository.ObtenerPorId(id, restauranteId)
                ?? throw new KeyNotFoundException("Reserva no encontrada.");
            reserva.Estado = nuevoEstado;
            _reservaRepository.Actualizar(reserva);
        }



        public async Task<ReservaDto?> ObtenerReservaPorCodeAsync(string code, Guid restauranteId)
        {
            var r = _reservaRepository.ObtenerPorCode(code, restauranteId);
            if (r == null) return null;

            return new ReservaDto
            {
                Id = r.Id,
                NombreCliente = r.NombreCliente,
                Email = r.Email,
                FechaReserva = r.FechaReserva,
                NumeroComensales = r.NumeroComensales,
                Notas = r.Notas,
                Estado = r.Estado.ToString(),
                TurnoId = r.TurnoId,
                Codigo = r.Codigo
            };
        }

        public async Task<bool> ActualizarReservaPorCodeAsync(string code, ReservaUpdateDto dto,Guid restauranteId)
        {
            var r = _reservaRepository.ObtenerPorCode(code, restauranteId);
            if (r == null) return false;
            r.FechaReserva = dto.FechaReserva;
            r.NumeroComensales = dto.NumeroComensales;
            _reservaRepository.Actualizar(r);
            await _reservaRepository.GuardarCambiosAsync();
            return true;
        }
        public async Task<bool> CancelarReservaPorCodeAsync(string code, Guid restauranteId)
        {
            var r = _reservaRepository.ObtenerPorCode(code, restauranteId);
            if (r == null) return false;
            r.Estado = Reserva.EstadoReserva.Cancelada;
            _reservaRepository.Actualizar(r);
            await _reservaRepository.GuardarCambiosAsync();
            return true;
        }

    }
}
