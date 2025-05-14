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
        
        Task<IEnumerable<Turno>> ObtenerTodosAsync(Guid restauranteId);
        Task<Turno?> ObtenerPorIdAsync(int id, Guid restauranteId);
        void Crear(Turno turno);
        void Actualizar(Turno turno);
        void Eliminar(Turno turno);

        Task GuardarCambiosAsync();

    }
}
