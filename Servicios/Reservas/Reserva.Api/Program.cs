using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Reservas.Application.Services;
using Reservas.Infrastructure.Data;
using Reservas.Infrastructure.Repositories;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ReservasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();




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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
