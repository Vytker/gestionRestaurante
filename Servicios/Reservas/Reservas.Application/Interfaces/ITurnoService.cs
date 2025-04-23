using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

namespace Reservas.Application.Interfaces
{
    public interface ITurnoService
    {
        Task<IEnumerable<Turno>> ObtenerTodosAsync();
        Task CrearTurnoAsync(TurnoCreateDto turnoDto);
        Task<IEnumerable<Turno>> ObtenerTodosTurnosAsync();
        Task<Turno?> ObtenerTurnoPorIdAsync(int id);
        Task<bool> EditarTurnoAsync(int id,TurnoUpdateDto dto);
        Task<bool> EliminarTurnoAsync(int id);



    }
}
