using Reservas.Domain.Entities;

namespace Reservas.Application.Interfaces
{
    public interface ITurnoRepository
    {
        // This interface is intended to define the contract for a repository that handles Turno entities.
        // It should include methods for CRUD operations and any other specific queries related to Turno.
        // For example:
        // Task<Turno> GetTurnoByIdAsync(int id);
        // Task<IEnumerable<Turno>> GetAllTurnosAsync();
        // Task AddTurnoAsync(Turno turno);
        // Task UpdateTurnoAsync(Turno turno);
        // Task DeleteTurnoAsync(int id);

        void Crear(Turno turno);
        Task GuardarCambiosAsync();
        Task<IEnumerable<Turno>> ObtenerTodosAsync();
        Task<Turno?> ObtenerPorIdAsync(int id);
        void Actualizar(Turno turno);
        Task<Turno> ObtenerPorIdAsync(int? id);
    }
}
