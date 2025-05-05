// Application/Services/UserService.cs
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using System.Security.Claims;
using System.Text;
using Identity.Application.Dtos;
using Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

using System.IdentityModel.Tokens.Jwt;


namespace Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IdentityDbContext _context;
        private readonly IConfiguration _config;

        public UserService(IdentityDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        
        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            // Verificar si el usuario ya existe
            if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            {
                return false; // Usuario ya existe
            }

            // Encriptar la contraseña
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Crear el usuario
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                Password = hashedPassword,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                Role = "User"
            };

            // Agregar el usuario a la base de datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            // Buscar el usuario por nombre de usuario
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return null; // Credenciales inválidas
            }

            // Generar el token JWT
            
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var ur = _context.UserRestaurantes
                             .FirstOrDefault(x => x.UserId == user.Id);


            if (user.Role != "SuperAdmin")
            {
                ur = _context.UserRestaurantes
                    .FirstOrDefault(x => x.UserId == user.Id) ?? throw new Exception("No se ha encontrado el restaurante del usuario");
            }
               

            var claims = new List<Claim>
            {
                // claim para user name
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, ur?.Role ?? user.Role ?? "User"),
                new Claim("restauranteId", ur?.RestaurantId.ToString() ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
