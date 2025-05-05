
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reservas.Application.Services;
using Reservas.Infrastructure.Data;
using Reservas.Infrastructure.Data.Repositories;
using Reservas.Infrastructure.Repositories;
using System.Text;
using Reservas.Application.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ReservasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();

builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();
builder.Services.AddScoped<ITurnoService, TurnoService>();

builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddControllers()
    .AddDataAnnotationsLocalization();  // no imprescindible, pero carga atributos

// Desarrollo: caché en memoria
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddTransient<INotificationService, NotificationService>();
// 1) Health checks básicos
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "SQL Server",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded)
    .AddCheck("self", () =>
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("OK"));

// 2) Health check UI (endpoints JSON)
builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

builder.Services.AddAutoQueryable(options =>
{
    options.DefaultPageSize = 10; // Tamaño de página por defecto
    options.MaxPageSize = 100; // Tamaño máximo de página
    options.DefaultOrderBy = "FechaReserva"; // Ordenación por defecto
    options.HandleNullPropagation = true; // Manejar valores nulos
});

// Configurar la autenticación con JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    
    // Configura los parámetros de validación (usa valores de configuración o hardcodea para pruebas)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
          policy
          .AllowAnyOrigin()   // URL de tu front
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReservasDbContext>();

    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("Conexión exitosa a la base de datos.");
        db.Database.CloseConnection();
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHsts();



app.UseCors("AllowFrontend");

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI(options => {
    options.UIPath = "/hc-ui";
    options.ApiPath = "/hc-api";
});



// Exponer métricas de HTTP (request count, latencias, etc.)
app.UseHttpMetrics();

// Mapea el endpoint /metrics
app.MapMetrics();


app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();

app.Run();
