using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

//Es la interfaz de repositorio que define qué operaciones se pueden hacer con la base de datos (sin saber cómo se hacen).
public interface IReservaRepository
{
    IEnumerable<Reserva> ObtenerTodas();                          // Para listar reservas
    Reserva? ObtenerPorId(Guid id);                                // Para buscar una reserva específica
    void Crear(Reserva reserva);                                   // Para añadir una nueva reserva
    Task<(bool Disponible, string? Error, string Code)> CrearReservaAsync(ReservaCreateDto dto); // Para crear una reserva y validar disponibilidad
    void Eliminar(Guid id);                                        // Para eliminar una reserva
    void EliminarReserva(Reserva reserva);                        // Para eliminar una reserva existente
    bool ExistePorCode(string code);
    Reserva? ObtenerPorCode(string code);
    void Actualizar(Reserva reserva);                              // Para modificar una reserva existente

    Task<Turno?> ObtenerTurnoConReservasPorIdAsync(int turnoId);   // Para validar disponibilidad del turno con sus reservas (esto es clave)
    Task GuardarCambiosAsync();
    void Eliminar(Reserva reserva);// Para confirmar los cambios en la BD
}

// La implementación real se hace luego en Infrastructure/Repositories/ReservaRepository.cs.
