using Reservas.Domain.Entities;
namespace Reservas.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyReservationCreatedAsync(Reserva reserva);
        //Esto permite cambiar el mecanismo (SMTP, SendGrid, WhatsApp…) sin tocar la lógica de negocio.
    }
}
