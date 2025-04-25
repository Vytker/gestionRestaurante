using Reservas.Application.Dtos;
using Reservas.Domain.Entities;

//Es la interfaz de repositorio que define qué operaciones se pueden hacer con la base de datos (sin saber cómo se hacen).
public interface IReservaRepository
{
    IEnumerable<Reserva> ObtenerTodas(Guid restauranteId);                          // Para listar reservas
    Reserva? ObtenerPorId(Guid id, Guid restauranteId);       
    
    bool ExistePorCode(string code, Guid restauranteId);
    Reserva? ObtenerPorCode(string code, Guid restauranteId);
                                                             
    //Turno para validar capacidad
    Task<Turno?> ObtenerTurnoConReservasPorIdAsync(int turnoId, Guid restauranteId);   // Para validar disponibilidad del turno con sus reservas (esto es clave)

    //Escritura
    void Crear(Reserva reserva);                                   // Para añadir una nueva reserva
    void Eliminar(Reserva reserva);                        // Para eliminar una reserva existente
    void Actualizar(Reserva reserva);                              // Para modificar una reserva existente

    
    Task GuardarCambiosAsync();



}

// La implementación real se hace luego en Infrastructure/Repositories/ReservaRepository.cs.
