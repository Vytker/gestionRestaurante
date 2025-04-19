using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

//Define los casos de uso (servicios de aplicación) que ofrece tu lógica de negocio.
// Esto es lo que llama el controlador (ReservasController), y se implementa en ReservaService.cs.

public interface IReservaService
{
    IEnumerable<Reserva> ObtenerTodasReservas();
    void ActualizarEstadoReserva(Guid id, Reserva.EstadoReserva nuevoEstado);
    
    Task<bool Disponible, string? Error> CrearReservaAsync(ReservaCreateDto reservaDto);
}
