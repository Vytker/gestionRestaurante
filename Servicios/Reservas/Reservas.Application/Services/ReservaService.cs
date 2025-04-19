using Microsoft.EntityFrameworkCore;
using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

namespace Reservas.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;

        public ReservaService(IReservaRepository reservaRepository)
        {
            _reservaRepository = reservaRepository;
        }


        public IEnumerable<Reserva> ObtenerTodasReservas()
        {
            return _reservaRepository.ObtenerTodas();
        }

        public async Task<(bool Disponible, string? Error)> CrearReservaAsync(ReservaCreateDto reservaDto)
        {
            var turno = await _reservaRepository.ObtenerTurnoConReservasPorIdAsync(reservaDto.TurnoId);
            //El método ObtenerTurnoConReservasPorIdAsync tiene como objetivo obtener un Turno con todas las Reservas asociadas a ese turno

            if (turno == null)
            {
                return (false, "Turno no encontrado.");
            }
            var comensalesYaReservados = turno.Reservas
                .Where(r => r.FechaReserva.Date == reservaDto.FechaReserva.Date)
                .Sum(r => r.NumeroComensales);

            if (comensalesYaReservados + reservaDto.NumeroComensales > turno.Capacidad)
            {
                return (false, "No hay suficiente capacidad en el turno seleccionado.");
            }
            var reserva = new Reserva
            {
                NombreCliente = reservaDto.NombreCliente,
                FechaReserva = reservaDto.FechaReserva,
                NumeroComensales = reservaDto.NumeroComensales,
                Notas = reservaDto.Notas,
                TurnoId = reservaDto.TurnoId
            };
            _reservaRepository.Crear(reserva);
            await _reservaRepository.GuardarCambiosAsync();
            return (true, null);
        }

        public void ActualizarEstadoReserva(Guid id, Reserva.EstadoReserva nuevoEstado)
        {
            var reserva = _reservaRepository.ObtenerPorId(id);
            if (reserva == null)
                throw new Exception("Reserva no encontrada.");

            reserva.Estado = nuevoEstado;
            _reservaRepository.Actualizar(reserva);
        }
    }
}
