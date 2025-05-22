
namespace Reservas.Application.Dtos
{
    public record SlotDto(
      int TurnoId,
      TimeSpan Hora,          // Hora de inicio del slot
      int PlazasDisponibles   // Capacidad restante para ese slot
        
  );
}
