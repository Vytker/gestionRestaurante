using Reservas.Application.Interfaces;
using Reservas.Domain.Entities;
using Microsoft.Extensions.Logging;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    public NotificationService(ILogger<NotificationService> logger)
        => _logger = logger;

    public Task NotifyReservationCreatedAsync(Reserva reserva)
    {
        _logger.LogInformation($"[SMS] Código {reserva.Codigo} enviado a {reserva.NombreCliente}");
        // Aquí integras Twilio o tu proveedor SMS/Email
        return Task.CompletedTask;
    }
}
