
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

        public IQueryable<Reserva> ObtenerTodas()
        => _reservaRepository.ObtenerTodas();
        public IQueryable<Reserva> ObtenerTodasReservas(Guid restauranteId)
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

        public async Task ActualizarEstadoReserva(Guid reservaId,
                                                Guid restauranteId,
                                                Reserva.EstadoReserva nuevoEstado)
        {
            var reserva = _reservaRepository.ObtenerPorId(reservaId, restauranteId)
                       ?? throw new KeyNotFoundException("Reserva no encontrada.");

            Console.WriteLine($"[Service] Reserva encontrada: ID={reserva.Id}, Estado actual={reserva.Estado}");

            if (reserva.Estado == nuevoEstado)
            {
                Console.WriteLine("[Service] El estado ya es el mismo. No se requiere actualización.");
                return;
            }

            reserva.Estado = nuevoEstado;
            Console.WriteLine($"[Service] Cambiando estado a {nuevoEstado}.");

            _reservaRepository.Actualizar(reserva);
            await _reservaRepository.GuardarCambiosAsync();
            Console.WriteLine("[Service] Cambios guardados en la base de datos.");

            // Envío de notificación
            await _notificationService.NotifyReservationStateChangedAsync(reserva);
        }
        public async Task ActualizarEstadoReservaSuperAdminAsync(Guid reservaId,
                                                         Reserva.EstadoReserva nuevoEstado)
        {
            // Recupera SIN filtrar por restaurante
            var reserva = _reservaRepository.ObtenerPorId(reservaId)
                       ?? throw new KeyNotFoundException("Reserva no encontrada.");

            if (reserva.Estado == nuevoEstado) return;

            reserva.Estado = nuevoEstado;
            _reservaRepository.Actualizar(reserva);
            await _reservaRepository.GuardarCambiosAsync();

            await _notificationService.NotifyReservationStateChangedAsync(reserva);
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
            

            var fechaDate = fecha.Date;

            var zona = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");
            if (fechaDate < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona).Date)
            {
                return Enumerable.Empty<SlotDto>(); // 0 slots

            }

            // 1) traemos todos los turnos activos
            var turnos =await _turnoRepository.ObtenerTodosAsync(restauranteId);
            var ahora = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona).TimeOfDay;
            var esHoy = fechaDate == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona).Date;

            var slots = new List<SlotDto>();

            foreach (var turno in turnos)
            {

                if (esHoy && turno.HoraInicio <= ahora)
                {
                    continue;
                }
                // 2) calculamos cuántos ya están reservados
                var reservados = turno.Reservas
                    .Where(r => r.FechaReserva.Date == fechaDate
                    && r.Estado != Reserva.EstadoReserva.Cancelada)
                    .Sum(r => r.NumeroComensales);

                var libres = turno.Capacidad - reservados;
                if (libres > 0)
                {
                    slots.Add(new SlotDto(
                        turno.Id,
                        turno.HoraInicio,
                        libres
                    ));
                }
            }

            return slots;
        }

        public async Task<int> ContarReservasAsync(
                                                    Guid restauranteId,
                                                    DateTime desde,
                                                    DateTime hasta,
                                                    string? estado = null)
        {
            var todas = await _reservaRepository.ObtenerPorRangoAsync(
                restauranteId, desde, hasta);

            if (!string.IsNullOrWhiteSpace(estado))
            {
                // Filtra por el estado que venga en string
                todas = todas.Where(r => r.Estado.ToString() == estado).ToList();
            }

            return todas.Count();
        }

        public async Task<IEnumerable<(DateTime Fecha, int Total)>> ObtenerSeriesDiariasAsync(
                                                                                            Guid restauranteId,
                                                                                            DateTime desde,
                                                                                            DateTime hasta,
                                                                                            string? estado = null)
        {
            var todas = await _reservaRepository.ObtenerPorRangoAsync(
                restauranteId, desde, hasta);

            if (!string.IsNullOrWhiteSpace(estado))
            {
                todas = todas.Where(r => r.Estado.ToString() == estado).ToList();
            }

            return todas
              .GroupBy(r => r.FechaReserva.Date)
              .Select(g => (Fecha: g.Key, Total: g.Count()))
              .OrderBy(x => x.Fecha);
        }


        // Nuevo: series horarias con filtro de estado
        public async Task<IEnumerable<HourlySeriesDto>> ObtenerSeriesHorariasAsync(
     Guid restauranteId,
     DateTime dia,
     string? estado = null)
        {
            // Normalizamos al rango [00:00 día … 00:00 día + 1)
            var inicio = dia.Date;
            var fin = inicio.AddDays(1);

            // Traemos todas las reservas en ese rango
            var todas = await _reservaRepository.ObtenerPorRangoAsync(restauranteId, inicio, fin);

            // Si llega un estado, filtramos
            if (!string.IsNullOrWhiteSpace(estado))
            {
                todas = todas.Where(r => r.Estado.ToString()
                                                .Equals(estado, StringComparison.OrdinalIgnoreCase))
                             .ToList();
            }

            // Agrupamos por hora y contamos
            var agrupado = todas
                .GroupBy(r => r.FechaReserva.Hour)
                .Select(g => new HourlySeriesDto(g.Key, g.Count()))
                .ToList();

            // Rellenamos horas faltantes con cero
            var completos = Enumerable.Range(0, 24)
                .Select(h => agrupado.FirstOrDefault(x => x.Hour == h)
                             ?? new HourlySeriesDto(h, 0))
                .ToList();

            return completos;
        }
    }
}
