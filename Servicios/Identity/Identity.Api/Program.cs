using HealthChecks.UI.Client;
using Identity.Application.Interfaces;
using Identity.Application.Services;
using Identity.Infrastructure;
using Identity.Infrastructure.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Security.Claims;
using System.Text;
using HealthChecks.NpgSql;
using Microsoft.OpenApi.Models;
using Amazon.ElasticMapReduce.Model;
using Identity.Application.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
//builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IUserService, UserService>();
// Configurar la autenticaci�n con JWT
builder.Services.AddScoped<IRestauranteService, RestauranteService>();
builder.Services.Configure<BrevoSettings>(builder.Configuration.GetSection("Brevo"));
builder.Services.AddHttpClient("Brevo", client =>
{
    client.BaseAddress = new Uri("https://api.brevo.com/v3/");
    client.DefaultRequestHeaders.Add("api-key", builder.Configuration["Brevo:ApiKey"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
//temporal quitar
var jwtKey = builder.Configuration["Jwt:Key"]!;

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("La clave JWT no se está leyendo!");
}

Console.WriteLine($"Clave JWT: {jwtKey} ({jwtKey.Length} caracteres)");


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  // "Bearer"
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendsPolicy", policy =>
        policy.AllowAnyOrigin() // Cambia esto a la URL de tu frontend
              .AllowAnyMethod()
              .AllowAnyHeader());
});


builder.Services.AddAuthorization();

builder.Services.AddControllers();

// 1) Health checks b�sicos
builder.Services.AddHealthChecks()
                .AddNpgSql(
                    builder.Configuration.GetConnectionString("DefaultConnection")!,
                    name: "PostgreSQL")
    .AddCheck("self", () =>
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("OK"));

// 2) Health check UI (endpoints JSON)
builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.WebHost.UseUrls("http://*:5000");


builder.Services.AddSwaggerGen(c =>
{
    // ① Definición del esquema de seguridad “Bearer”
    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Pegue **solo** el token (sin la palabra Bearer)",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"          // ← la clave
        }
    };
    c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);

    // ② Requisito global: todas las operaciones usarán el esquema
    var requirement = new OpenApiSecurityRequirement
    {
        [jwtScheme] = Array.Empty<string>()
    };
    c.AddSecurityRequirement(requirement);
});

var app = builder.Build();
app.UseCors("AllowAll");
// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("Conexi�n exitosa a la base de datos.");
        db.Database.CloseConnection();
    }
    catch (PostgresException ex)
    {
        Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
    }
}


// app.UseHttpsRedirection();
app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI(options => {
    options.UIPath = "/hc-ui";
    options.ApiPath = "/hc-api";
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();


app.UseCors("FrontendsPolicy");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    db.Database.Migrate();
    await IdentitySeeder.SeedAsync(db, app.Configuration);
}

app.Run();