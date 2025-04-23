using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

// Define los casos de uso (servicios de aplicación) que ofrece tu lógica de negocio.
// Esto es lo que llama el controlador (ReservasController), y se implementa en ReservaService.cs.

public interface IReservaService
{
    IEnumerable<Reserva> ObtenerTodasReservas();
    void ActualizarEstadoReserva(Guid id, Reserva.EstadoReserva nuevoEstado);

    // Corrección: Cambiar la firma del método para usar un tipo de retorno genérico adecuado.
    Task<(bool Disponible, string? Error, string Codigo)> CrearReservaAsync(ReservaCreateDto reservaDto);
    Task<ReservaDto?> ObtenerReservaPorCodeAsync(string code);
    Task<bool> ActualizarReservaPorCodeAsync(string code, ReservaUpdateDto dto);
    Task<bool> CancelarReservaPorCodeAsync(string code);
    //existe por codigo
    bool ExistePorCode(string code);
    // Obtener reserva por código

}
