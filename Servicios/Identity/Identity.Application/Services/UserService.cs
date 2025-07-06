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
using Identity.Application.Common;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;



namespace Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IdentityDbContext _context;
        private readonly IConfiguration _config;
        private readonly BrevoSettings _brevo;
        private readonly IHttpClientFactory _httpFactory;
        public UserService(IdentityDbContext context, IConfiguration config, IHttpClientFactory httpFactory, IOptions<BrevoSettings> brevoOpt)
        {
            _context = context;
            _config = config;
            _brevo = brevoOpt.Value;
            _httpFactory = httpFactory;
        }
        

        public async Task<OperationResult> InviteAsync(RegisterDto dto, Guid creatorId)
        {

            // Verificar si el creador es Owner o SuperAdmin


            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.UserName) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.PasswordConfirm))
            {
                return OperationResult.Fail("Email, UserName y contraseña son requeridos.");
            }
            if (dto.Password != dto.PasswordConfirm)
                return OperationResult.Fail("Las contraseñas no coinciden.");
            if (!PasswordPolicy.IsValid(dto.Password!))
                return OperationResult.Fail("La contraseña no cumple la política.");
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return OperationResult.Fail("Email ya en uso.");
            if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
                return OperationResult.Fail("UserName ya en uso.");

            // Crear usuario con IsFirstLogin = true
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName!,
                Email = dto.Email!,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password!),
                Role = dto.Role!,
                IsFirstLogin = true,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);

            // Determinar restaurantId: claim o dto.RestaurantId
            Guid restaurantId;
            if (dto.RestaurantId.HasValue)
            {
                restaurantId = dto.RestaurantId.Value;
            }
            else
            {
                restaurantId = await _context.UserRestaurantes
                    .Where(ur => ur.UserId == creatorId)
                    .Select(ur => ur.RestaurantId)
                    .FirstOrDefaultAsync();
            }
            // Asociar al restaurante
            _context.Attach(new Restaurante { Id = restaurantId });
            _context.UserRestaurantes.Add(new UserRestaurante
            {
                UserId = user.Id,
                RestaurantId = restaurantId,
                Role = dto.Role!
            });

           

            // Generar token de invitación
            var token = Guid.NewGuid().ToString("N");
            _context.Invitations.Add(new Invitation
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsUsed = false
            });

            await _context.SaveChangesAsync();


            // TODO: enviar por email el `token`

            var frontUrl = _config["Frontend:BaseUrl"];
            var link = $"{frontUrl}/complete-profile?token={token}";
            await SendInvitationEmail(dto.Email!, dto.UserName!, link);

            return OperationResult.Ok();
        }
        private async Task SendInvitationEmail(string toEmail, string toName, string link)
        {
            var client = _httpFactory.CreateClient("Brevo");

            // payload según la API SMTP de Brevo
            var payload = new
            {
                sender = new { name = _brevo.FromName, email = _brevo.FromEmail },
                to = new[] { new { email = toEmail, name = toName } },
                subject = "¡Estás invitado a la plataforma de restaurantes!",
                htmlContent = $@"
                <p>Hola {toName},</p>
                <p>Has sido invitado a unirte a nuestro portal de restaurantes.</p>
                <p>Para completar tu registro y definir tu contraseña definitiva, haz clic aquí:</p>
                <p><a href=""{link}"">Completar perfil</a></p>
                <br/>
                <p>Si no esperabas este email, ignóralo.</p>"
            };

            var resp = await client.PostAsJsonAsync("smtp/email", payload);
            resp.EnsureSuccessStatusCode();
        }

        // 2) Completar perfil en primer login
        public async Task<OperationResult> CompleteInvitationAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.InviteToken) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.PasswordConfirm) ||
                string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName))
            {
                return OperationResult.Fail("Token, contraseña, nombre y apellidos son requeridos.");
            }

            // Buscar invitación válida
            var inv = await _context.Invitations
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Token == dto.InviteToken && !i.IsUsed);
            if (inv == null || inv.ExpiresAt < DateTime.UtcNow)
                return OperationResult.Fail("Invitación inválida o expirada.");

            if (dto.Password != dto.PasswordConfirm)
                return OperationResult.Fail("Las contraseñas no coinciden.");
            if (!PasswordPolicy.IsValid(dto.Password!))
                return OperationResult.Fail("La contraseña no cumple la política.");

            // Actualizar usuario
            var user = inv.User!;
            if (await _context.Users
       .AnyAsync(u => u.UserName == dto.UserName && u.Id != user.Id))
            {
                return OperationResult.Fail("El nombre de usuario ya está en uso.");
            }
            if (!string.IsNullOrWhiteSpace(dto.UserName))
                user.UserName = dto.UserName;
            user.FirstName = dto.FirstName!;
            user.LastName = dto.LastName!;
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password!);
            user.IsFirstLogin = false;
            user.LastUpdatedAt = DateTime.UtcNow;

            // Marcar invitación como usada
            inv.IsUsed = true;

            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            // Verificar si el usuario ya existe
            if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            {
                return false; // Usuario ya existe
            }
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return false; // Usuario ya existe
            }

            // Encriptar la contraseña
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Crear el usuario
            var user = new Domain.Entities.User
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

        private string GenerateJwtToken(Domain.Entities.User user)
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

        /* ---------- PERFIL ---------- */
        public async Task<UserProfileDto?> GetProfileAsync(Guid id)
        {
            var u = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return u == null
                ? null
                : new UserProfileDto(
                    u.Id,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Telefono ?? string.Empty // Ensure Telefono is not null
                );
        }

        // Replacing the line causing the error in the ChangePasswordAsync method

        public async Task<bool> UpdateProfileAsync(Guid id, UpdateProfileDto dto)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return false;

            u.FirstName = dto.FirstName ?? u.FirstName;
            u.LastName = dto.LastName ?? u.LastName;
            u.Telefono = dto.Telefono ?? u.Telefono;
            u.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ChangePasswordAsync(Guid id, string current, string next)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return false;
            if (!BCrypt.Net.BCrypt.Verify(current, u.Password)) return false;

            u.Password = BCrypt.Net.BCrypt.HashPassword(next);
            u.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
       
    }
}
