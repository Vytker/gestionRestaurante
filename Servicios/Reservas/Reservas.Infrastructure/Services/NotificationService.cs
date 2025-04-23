using MailKit.Net.Smtp;
using MimeKit;
using Reservas.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Reservas.Application.Interfaces;

public class NotificationService : INotificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IConfiguration config, ILogger<NotificationService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task NotifyReservationCreatedAsync(Reserva reserva)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Restaurant", _config["Email:From"]));
        message.To.Add(new MailboxAddress(reserva.NombreCliente, reserva.Email));
        message.Subject = "Confirmación de reserva";

        message.Body = new TextPart("plain")
        {
            Text =
$@"Hola {reserva.NombreCliente},

Hemos recibido tu reserva para el {reserva.FechaReserva:dd/MM/yyyy HH:mm}.
Tu código de seguimiento es {reserva.Codigo}.

¡Gracias!"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]), false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Correo de confirmación enviado a {Email}", reserva.Email);
    }
}
