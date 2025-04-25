using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

// Define los casos de uso (servicios de aplicación) que ofrece tu lógica de negocio.
// Esto es lo que llama el controlador (ReservasController), y se implementa en ReservaService.cs.

public interface IReservaService
{
    IEnumerable<Reserva> ObtenerTodasReservas(Guid restauranteId);
    void ActualizarEstadoReserva(Guid restauranteId,Guid id, Reserva.EstadoReserva nuevoEstado);

    // Corrección: Cambiar la firma del método para usar un tipo de retorno genérico adecuado.
    Task<(bool Disponible, string? Error, string Codigo)> CrearReservaAsync(Guid restauranteId,ReservaCreateDto reservaDto);
    Task<ReservaDto?> ObtenerReservaPorCodeAsync(string code, Guid restauranteId);
    Task<bool> ActualizarReservaPorCodeAsync(string code, ReservaUpdateDto dto, Guid restauranteId);
    Task<bool> CancelarReservaPorCodeAsync(string code, Guid restauranteId);
    //existe por codigo


}
