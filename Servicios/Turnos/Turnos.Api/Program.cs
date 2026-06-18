using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Turnos.Infrastructure.Persistence;
using Turnos.Infrastructure.Repositories;
using MediatR;
using Turnos.Application.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Turnos.Application.Behaviors;


var builder = WebApplication.CreateBuilder(args);

// 1. Leer cadena de conexi�n
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar DbContext
builder.Services.AddDbContext<TurnosDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();

// 4. Registrar MediatR (busca handlers en el assembly de Application)

var myAllowedOrigins = new[] { "http://127.0.0.1:8002" }; // front en Laravel
// 1) Definir la pol�tica CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminPortal", policy =>
    {
        policy
          .WithOrigins(myAllowedOrigins)   // tu portal admin
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
    });
});


builder.Services.AddMediatR(cfg =>
{
    
    cfg.RegisterServicesFromAssemblyContaining<CreateSlotCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<DeleteAssignmentCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetAllShiftsQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetAllSlotsQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<CreateAssignmentCommand>();
    cfg.RegisterServicesFromAssemblyContaining<GetAssignmentsByDateQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<CreateShiftCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<DeleteShiftCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<UpdateShiftCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetShiftsByEmployeeQueryHandler>();

    cfg.AddOpenBehavior(typeof(TenantResolutionBehavior<,>));
});

// 3. Registrar repositorio (inversi�n de control)
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();

builder.Services.AddAuthentication(options =>
{
    // Este es el esquema �por defecto� que ASP usa para [Authorize]
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // en dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});

builder.Services.AddAuthorization();
// 5. Registrar controladores y Swagger (opcional)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Turnos API",
        Version = "v1",
        Description = "Microservicio de gesti�n de turnos"
    });
});

var app = builder.Build();

app.UseCors("AllowAdminPortal");

// 6. Middleware: Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// 7. Middleware comunes
app.UseHttpsRedirection();
app.UseAuthentication();  // si usas autenticaci�n
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TurnosDbContext>();
    db.Database.Migrate();
    Console.WriteLine("Migraciones aplicadas correctamente.");
}

app.Run();
