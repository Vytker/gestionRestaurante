
using Reservas.Application.Dtos;
using Reservas.Application.Interfaces;
using Reservas.Domain.Entities;

namespace Reservas.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly ITurnoRepository _turnoRepository;
        private readonly INotificationService _notificationService;
        private IReservaRepository object1;
        private INotificationService object2;

        public ReservaService(
            IReservaRepository reservaRepository,
            INotificationService notificationService,
            ITurnoRepository turnoRepository)
        {
            _reservaRepository = reservaRepository;
            _notificationService = notificationService;
            _turnoRepository = turnoRepository;
        }

        public ReservaService(IReservaRepository object1, INotificationService object2)
        {
            this.object1 = object1;
            this.object2 = object2;
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

        public async Task ActualizarEstadoReserva(Guid id, Guid restauranteId,Reserva.EstadoReserva nuevoEstado)
        {
            var reserva = _reservaRepository.ObtenerPorId(id, restauranteId)
                ?? throw new KeyNotFoundException("Reserva no encontrada.");
            Console.WriteLine($"[Service] Reserva encontrada: ID={reserva.Id}, Estado actual={reserva.Estado}");
            

            // Verificar si el estado ya es el mismo
            if (reserva.Estado == nuevoEstado)
            {
                Console.WriteLine("[Service] El estado ya es el mismo. No se requiere actualización.");
                return;
            }
            
            reserva.Estado = nuevoEstado;
            Console.WriteLine($"[Service] Cambiando estado de {reserva.Estado} a {nuevoEstado}.");

            _reservaRepository.Actualizar(reserva);
            Console.WriteLine($"[Service] Estado actualizado a: {nuevoEstado}");

            await _reservaRepository.GuardarCambiosAsync();
            Console.WriteLine("[Service] Cambios guardados en la base de datos.");

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

        public async Task<IEnumerable<SlotDto>> ObtenerSlotsDisponiblesAsync(Guid restauranteId, DateTime fecha)
        {
            // 1) traemos todos los turnos activos
            var turnos =await _turnoRepository.ObtenerTodosAsync(restauranteId);

            var fechaDate = fecha.Date;
            var slots = new List<SlotDto>();

            foreach (var turno in turnos)
            {
                // 2) calculamos cuántos ya están reservados
                var reservados = turno.Reservas
                    .Where(r => r.FechaReserva.Date == fechaDate
                    && r.Estado != Reserva.EstadoReserva.Cancelada)
                    .Sum(r => r.NumeroComensales);

                var libres = turno.Capacidad - reservados;
                if (libres > 0)
                {
                    slots.Add(new SlotDto(
                        TurnoId: turno.Id,
                        Hora: turno.HoraInicio,
                        PlazasDisponibles: libres
                    ));
                }
            }

            return slots;
        }


    }
}
