using Reservas.Application.Dtos;
using Reservas.Application.Interfaces;
using Reservas.Domain.Entities;


namespace Reservas.Application.Services
{
    public class TurnoService : ITurnoService
    {
        private readonly ITurnoRepository _turnoRepository;
        public TurnoService(ITurnoRepository turnoRepository)
        {
            _turnoRepository = turnoRepository;
        }

        public async Task<IEnumerable<Turno>> ObtenerTodosAsync(Guid restauranteId)
        {
            return await _turnoRepository.ObtenerTodosAsync(restauranteId);
        }
        public async Task<Turno?> ObtenerTurnoPorIdAsync(int id, Guid restauranteId)
        {
            return await _turnoRepository.ObtenerPorIdAsync(id, restauranteId);
        }

        public async Task CrearTurnoAsync(TurnoCreateDto turnoDto, Guid restauranteId)
        {
            if(string.IsNullOrWhiteSpace(turnoDto.Nombre))
            {
                throw new ArgumentException("El nombre del turno no puede estar vacío.");
            }
            if (turnoDto.Capacidad <= 0)
            {
                throw new ArgumentException("La capacidad del turno debe ser mayor a cero.");
            }
            if (turnoDto.HoraInicio >= turnoDto.HoraFin)
            {
                throw new ArgumentException("La hora de inicio debe ser menor que la hora de fin.");
            }
            // Verificar si ya existe un turno con el mismo nombre
            var turnosExistentes = (await _turnoRepository.ObtenerTodosAsync(restauranteId))
                                    .Where(t => !t.Eliminado);

            var solapado = turnosExistentes.Any(t => t.Nombre == turnoDto.Nombre &&
                ((turnoDto.HoraInicio >= t.HoraInicio && turnoDto.HoraInicio < t.HoraFin) ||
                (turnoDto.HoraFin > t.HoraInicio && turnoDto.HoraFin <= t.HoraFin)));


            if (solapado)
                throw new InvalidOperationException("Ya existe un turno con el mismo nombre y horario.");

            // Crear el nuevo turno
            var turno = new Turno
            {
                RestauranteId = restauranteId,
                Nombre = turnoDto.Nombre,
                Capacidad = turnoDto.Capacidad,
                HoraInicio = turnoDto.HoraInicio,
                HoraFin = turnoDto.HoraFin
            };
            _turnoRepository.Crear(turno);
            await _turnoRepository.GuardarCambiosAsync();
        }


        public async Task<bool> EditarTurnoAsync(int id,TurnoUpdateDto dto, Guid restauranteId)
        {

            var turno = await _turnoRepository.ObtenerPorIdAsync(id, restauranteId);

            if (turno == null)
                throw new Exception("Turno no encontrado.");

            if (dto.Nombre != null && string.IsNullOrWhiteSpace(dto.Nombre))
            {
                throw new ArgumentException("El nombre del turno no puede estar vacío.");
            }
            if (dto.Capacidad != null && dto.Capacidad <= 0)
            {
                throw new ArgumentException("La capacidad del turno debe ser mayor a cero.");
            }
            if (dto.HoraInicio >= dto.HoraFin) // Convert DateTime to TimeSpan
            {
                throw new ArgumentException("La hora de inicio debe ser menor que la hora de fin.");
            }

            if (dto.HoraInicio is not null || dto.HoraFin is not null)
            {
                var otros = (await _turnoRepository.ObtenerTodosAsync(restauranteId))
                            .Where(t => !t.Eliminado);
                var solapado1 = otros.Where(t => t.Id != dto.Id)
                    .Any(t =>
                        (dto.HoraInicio ?? turno.HoraInicio) < t.HoraFin &&
                        (dto.HoraFin ?? turno.HoraFin) > t.HoraInicio);

                if (solapado1)
                    throw new InvalidOperationException("El horario se solapa con otro turno existente.");
            }

            //validar solapamiento con otros turnos (excluyendo el actual)
            var otrosTurnos = await _turnoRepository.ObtenerTodosAsync(restauranteId);
            var solapado = otrosTurnos.Where(t => t.Id != dto.Id).Any(t => dto.HoraInicio < t.HoraFin && dto.HoraFin > t.HoraInicio);

            if (solapado)
                throw new InvalidOperationException("El horario se solapa con otro turno existente.");
            //actualizar campos
            if(dto.Nombre is not null)
            {
                turno.Nombre = dto.Nombre!;
            }
            if (dto.Capacidad is not null)
            {
                turno.Capacidad = dto.Capacidad.Value;
            }
            if (dto.HoraInicio is not null)
            {
                turno.HoraInicio = dto.HoraInicio.Value;
            }
            if (dto.HoraFin is not null)
            {
                turno.HoraFin = dto.HoraFin.Value;
            }

            _turnoRepository.Actualizar(turno);
            await _turnoRepository.GuardarCambiosAsync();
            return true; // Add return statement to complete the method
        }

        public async Task<bool> EliminarTurnoAsync(int id, Guid restauranteId)
        {
            var turno = await _turnoRepository.ObtenerPorIdAsync(id, restauranteId);
            if (turno == null || turno.Eliminado)
                throw new Exception("Turno no encontrado o eliminado.");
            // Marcar como eliminado
            turno.Eliminado = true;
            _turnoRepository.Actualizar(turno);
            await _turnoRepository.GuardarCambiosAsync();
            return true;
        }
        public async Task<IEnumerable<SlotDto>> ObtenerSlotsDisponiblesAsync(Guid restauranteId, DateTime fecha)
        {
            // 1) Traer todos los turnos no eliminados del restaurante
            var turnos = await _turnoRepository.ObtenerTodosAsync(restauranteId); // Await the Task to get the actual IEnumerable<Turno>

            var fechaDate = fecha.Date;
            var slots = new List<SlotDto>();

            foreach (var turno in turnos) // Now 'turnos' is an IEnumerable<Turno>
            {
                // 2) Calcular cuántos comensales ya hay reservados en ese turno y fecha
                var reservados = turno.Reservas
                    .Where(r => r.FechaReserva.Date == fechaDate)
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
    }
}
