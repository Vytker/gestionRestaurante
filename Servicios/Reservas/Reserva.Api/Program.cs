
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
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData.Routing.Attributes;

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
    .AddDataAnnotationsLocalization()  // no imprescindible, pero carga atributos
    .AddOData(opt =>
    {
        var odataBuilder = new ODataConventionModelBuilder();
        odataBuilder.EntitySet<Reservas.Domain.Entities.Reserva>("Reservas"); 
        opt.AddRouteComponents("api/odata", odataBuilder.GetEdmModel())
            .Select()
            .Filter()
            .OrderBy()
            .Expand()
            .Count()
            .SetMaxTop(100); // m�ximo 100 elementos por p�gina
        
        opt.EnableQueryFeatures();
        
    });
Console.WriteLine("Modelo EDM generado correctamente.");
// Desarrollo: cach� en memoria
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddTransient<INotificationService, NotificationService>();
// 1) Health checks b�sicos
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



// Configurar la autenticaci�n con JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    
    // Configura los par�metros de validaci�n (usa valores de configuraci�n o hardcodea para pruebas)
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


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReservasDbContext>();
    db.Database.Migrate();
    Console.WriteLine("Migraciones aplicadas correctamente.");
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



// Exponer m�tricas de HTTP (request count, latencias, etc.)
app.UseHttpMetrics();

// Mapea el endpoint /metrics
app.MapMetrics();


app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
