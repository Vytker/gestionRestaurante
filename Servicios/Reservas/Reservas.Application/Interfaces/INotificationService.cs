using Reservas.Domain.Entities;
namespace Reservas.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyReservationCreatedAsync(Reserva reserva);
    }
}
