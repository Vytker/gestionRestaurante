using Reservas.Domain.Entities;

//Es la interfaz de repositorio que define qué operaciones se pueden hacer con la base de datos (sin saber cómo se hacen).
public interface IReservaRepository
{
    IEnumerable<Reserva> ObtenerTodas();                          // Para listar reservas
    Reserva? ObtenerPorId(Guid id);                                // Para buscar una reserva específica
    void Crear(Reserva reserva);                                   // Para añadir una nueva reserva
    void Actualizar(Reserva reserva);                              // Para modificar una reserva existente

    Task<Turno?> ObtenerTurnoConReservasPorIdAsync(int turnoId);   // Para validar disponibilidad del turno con sus reservas (esto es clave)
    Task GuardarCambiosAsync();                                    // Para confirmar los cambios en la BD
}

// La implementación real se hace luego en Infrastructure/Repositories/ReservaRepository.cs.
