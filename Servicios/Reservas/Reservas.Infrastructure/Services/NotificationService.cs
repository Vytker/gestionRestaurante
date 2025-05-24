using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Encodings;
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

    public NotificationService(
        IConfiguration config,
        ILogger<NotificationService> logger,
        IMemoryCache cache)
    {
        _config = config;
        _logger = logger;
        _cache = cache;
    }

    public async Task NotifyReservationCreatedAsync(Reserva reserva)
    {
        // --- 1) Obtener URL con fallback ---
        var cacheKey = $"RestaurantBaseUrl_{reserva.RestauranteId}";
        // Intentamos cachear solo si existe la URL
        string maybeUrl = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return _config.GetSection("RestaurantUrls")[reserva.RestauranteId.ToString()];
        });

        if (string.IsNullOrWhiteSpace(maybeUrl))
        {
            _logger.LogWarning(
                "Tenant {Tenant} sin URL configurada; no se incluirá link en el e-mail",
                reserva.RestauranteId);
            maybeUrl = null;
        }

        // --- 2) Obtener e-mail restaurante con fallback ---
        var restEmail = _config
            .GetSection("RestaurantEmails")[reserva.RestauranteId.ToString()];

        if (string.IsNullOrWhiteSpace(restEmail))
        {
            restEmail = _config["Email:FallbackNotificationEmail"];
            _logger.LogWarning(
                "No se encontró RestaurantEmails[{Tenant}]; usando fallback {Email}",
                reserva.RestauranteId, restEmail);
        }

        // --- 3) Construir cuerpo dinámico ---
        string cuerpoPlano;
        if (maybeUrl is null)
        {
            cuerpoPlano =
$@"Hola {reserva.NombreCliente},

Hemos recibido tu reserva para el {reserva.FechaReserva:dd/MM/yyyy HH:mm}.
Tu código de seguimiento es {reserva.Codigo}.

¡Gracias!";
        }
        else
        {
            var link = $"{maybeUrl.TrimEnd('/')}/track?code={reserva.Codigo}";
            cuerpoPlano =
$@"Hola {reserva.NombreCliente},

Hemos recibido tu reserva para el {reserva.FechaReserva:dd/MM/yyyy HH:mm}.
Tu código de seguimiento es {reserva.Codigo}.

Puedes ver el estado de tu reserva en:
{link}

¡Gracias!";
        }

        // --- 4) Crear y enviar mensaje ---
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Restaurant", _config["Email:From"]));
        message.To.Add(new MailboxAddress(reserva.NombreCliente, reserva.Email));
        message.Bcc.Add(new MailboxAddress("Restaurante", restEmail));
        message.ReplyTo.Add(new MailboxAddress("Restaurante", restEmail));
        message.Subject = "Confirmación de reserva";
        message.Body = new TextPart("plain") { Text = cuerpoPlano };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(
                _config["Email:SmtpHost"],
                int.Parse(_config["Email:SmtpPort"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _config["Email:Username"],
                _config["Email:Password"]);

            await client.SendAsync(message);
            _logger.LogInformation(
                "Correo de confirmación enviado a {Cliente} y copia a {Rest}",
                reserva.Email, restEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al enviar e-mail de reserva creada para {ReservaId}",
                reserva.Id);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }

    public async Task NotifyReservationStateChangedAsync(Reserva reserva)
    {
        // --- 1) Obtener URL con fallback ---
        var cacheKey = $"RestaurantBaseUrl_{reserva.RestauranteId}";
        string maybeUrl = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return _config.GetSection("RestaurantUrls")[reserva.RestauranteId.ToString()];
        });

        if (string.IsNullOrWhiteSpace(maybeUrl))
        {
            _logger.LogWarning(
                "Tenant {Tenant} sin URL configurada; notificación sin link",
                reserva.RestauranteId);
            maybeUrl = null;
        }

        // --- 2) Construir asunto y cuerpo según estado ---
        (string subject, string body) = reserva.Estado switch
        {
            Reserva.EstadoReserva.Confirmada => (
                "Tu reserva ha sido confirmada",
$@"Hola {reserva.NombreCliente},

Tu reserva para {reserva.FechaReserva:dd/MM/yyyy HH:mm} ha sido *Confirmada*.
Código de seguimiento: {reserva.Codigo}
{(maybeUrl is null
    ? string.Empty
    : $"\nPuedes verlo en: {maybeUrl.TrimEnd('/')}/track?code={reserva.Codigo}\n")}"
            ),
            Reserva.EstadoReserva.Cancelada => (
                "Tu reserva ha sido cancelada",
$@"Hola {reserva.NombreCliente},

Lamentamos informarte que tu reserva para {reserva.FechaReserva:dd/MM/yyyy HH:mm} ha sido *Cancelada*.
Código de seguimiento: {reserva.Codigo}
{(maybeUrl is null
    ? string.Empty
    : $"\nMás info en: {maybeUrl.TrimEnd('/')}/track?code={reserva.Codigo}\n")}"
            ),
            Reserva.EstadoReserva.Rechazada => (
                "Tu reserva ha sido rechazada",
$@"Hola {reserva.NombreCliente},

Lamentamos informarte que tu reserva para {reserva.FechaReserva:dd/MM/yyyy HH:mm} ha sido *Rechazada*.
Código de seguimiento: {reserva.Codigo}
{(maybeUrl is null
    ? string.Empty
    : $"\nMás info en: {maybeUrl.TrimEnd('/')}/track?code={reserva.Codigo}\n")}"
            ),
            _ => (null, null)
        };

        if (subject is null) return;

        // --- 3) Obtener e-mail restaurante con fallback ---
        var restEmail = _config
            .GetSection("RestaurantEmails")[reserva.RestauranteId.ToString()];
        if (string.IsNullOrWhiteSpace(restEmail))
        {
            restEmail = _config["Email:FallbackNotificationEmail"];
            _logger.LogWarning(
                "No se encontró email para tenant {Tenant}, usando fallback {Email}",
                reserva.RestauranteId, restEmail);
        }

        // --- 4) Crear y enviar mensaje ---
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Restaurant", _config["Email:From"]));
        message.To.Add(new MailboxAddress(reserva.NombreCliente, reserva.Email));
        message.Bcc.Add(new MailboxAddress("Restaurante", restEmail));
        message.ReplyTo.Add(new MailboxAddress("Restaurante", restEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = body,
            ContentTransferEncoding = ContentEncoding.Base64
        };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(
                _config["Email:SmtpHost"],
                int.Parse(_config["Email:SmtpPort"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _config["Email:Username"],
                _config["Email:Password"]);

            await client.SendAsync(message);
            _logger.LogInformation(
                "Correo de estado {Estado} enviado a {Cliente} y copia a {Rest}",
                reserva.Estado, reserva.Email, restEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al enviar e-mail de cambio de estado para {ReservaId}",
                reserva.Id);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
