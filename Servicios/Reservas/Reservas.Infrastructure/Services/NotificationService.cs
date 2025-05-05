using MailKit.Net.Smtp;
using MimeKit;
using Reservas.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Reservas.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;


public class NotificationService : INotificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<NotificationService> _logger;
    private readonly IMemoryCache _cache;

    public NotificationService(IConfiguration config, ILogger<NotificationService> logger, IMemoryCache cache)
    {
        _config = config;
        _logger = logger;
        _cache = cache;
    }

    public async Task NotifyReservationCreatedAsync(Reserva reserva)
    { // 3) Recuperar (o crear) la URL base desde el cache
        var cacheKey = $"RestaurantBaseUrl_{reserva.RestauranteId}";
        var baseUrl = _cache.GetOrCreate(cacheKey, entry =>
        {
            // opcional: expiración
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            // Suponemos que en appsettings.json tienes:
            // "RestaurantUrls": {
            //    "74538fad-dd1f-40e2-86d1-0d9b86442d07": "https://mullat.app",
            //    ...
            // }
            var section = _config.GetSection("RestaurantUrls");
            return section[reserva.RestauranteId.ToString()]
                   ?? throw new InvalidOperationException($"No hay URL configurada para el restaurante {reserva.RestauranteId}");
        });

        // 4) Construir el enlace de seguimiento
        var trackingLink = $"{baseUrl.TrimEnd('/')}/track?code={reserva.Codigo}";

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

Puedes ver el estado de tu reserva en:
{trackingLink}

¡Gracias!"
        };


        using var client = new SmtpClient();
        await client.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]), false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Correo de confirmación enviado a {Email} con link {Link}", reserva.Email, trackingLink);
    }
}
