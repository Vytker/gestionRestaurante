using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

namespace Reservas.Application.Interfaces
{
    public interface ITurnoService
    {
        Task<IEnumerable<Turno>> ObtenerTodosAsync(Guid? restauranteId);
        Task<IEnumerable<TurnoDto>> ObtenerTurnosAsync(Guid? restauranteId);
        Task CrearTurnoAsync(TurnoCreateDto turnoDto, Guid restauranteId);
        Task<Turno?> ObtenerTurnoPorIdAsync(int id, Guid restauranteId);
        Task<bool> EditarTurnoAsync(int id,TurnoUpdateDto dto, Guid restauranteId);
        Task<bool> EliminarTurnoAsync(int id, Guid restauranteId);

        
        /// Devuelve, para una fecha dada, todos los turnos con su capacidad restante (slots).
        Task<IEnumerable<SlotDto>> ObtenerSlotsDisponiblesAsync(Guid restauranteId, DateTime fecha);


    }
}
