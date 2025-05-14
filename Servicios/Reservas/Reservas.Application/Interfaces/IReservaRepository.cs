using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

//Es la interfaz de repositorio que define qué operaciones se pueden hacer con la base de datos (sin saber cómo se hacen).
public interface IReservaRepository
{
    IQueryable<Reserva> ObtenerTodas();
    IQueryable<Reserva> ObtenerTodas(Guid restauranteId);                          // Para listar reservas
    Reserva? ObtenerPorId(Guid id, Guid restauranteId);
    Reserva? ObtenerPorId(Guid id);
    bool ExistePorCode(string code, Guid restauranteId);
    Reserva? ObtenerPorCode(string code, Guid restauranteId);
                                                             
    //Turno para validar capacidad
    Task<Turno?> ObtenerTurnoConReservasPorIdAsync(int turnoId, Guid restauranteId);   // Para validar disponibilidad del turno con sus reservas (esto es clave)

    //Escritura
    void Crear(Reserva reserva);                                   // Para añadir una nueva reserva
    void Eliminar(Reserva reserva);                        // Para eliminar una reserva existente
    void Actualizar(Reserva reserva);                              // Para modificar una reserva existente

    
    Task GuardarCambiosAsync();

    Task<IEnumerable<Reserva>> ObtenerPorRangoAsync(Guid restauranteId, DateTime desde, DateTime hasta);

}

// La implementación real se hace luego en Infrastructure/Repositories/ReservaRepository.cs.
