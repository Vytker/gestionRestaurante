using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Turnos.Infrastructure.Persistence;
using Turnos.Infrastructure.Repositories;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// 1. Leer cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar DbContext
builder.Services.AddDbContext<TurnosDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Registrar repositorio (inversión de control)
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();

// 4. Registrar MediatR (busca handlers en el assembly de Application)
builder.Services.AddMediatR(typeof(Turnos.Application.Queries.GetShiftsByEmployeeQuery).Assembly);

// 5. Registrar controladores y Swagger (opcional)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Turnos API",
        Version = "v1",
        Description = "Microservicio de gestión de turnos"
    });
});

var app = builder.Build();

// 6. Middleware: Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 7. Middleware comunes
app.UseHttpsRedirection();
app.UseAuthentication();  // si usas autenticación
app.UseAuthorization();

app.MapControllers();

app.Run();
