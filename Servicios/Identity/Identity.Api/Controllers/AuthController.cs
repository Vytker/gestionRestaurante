
using Microsoft.AspNetCore.Mvc;
using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto);
            if(token == null)
            {
                return Unauthorized("Credenciales inválidas");
            }
            return Ok(new { token });   
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
           var success = await _userService.RegisterAsync(dto);
            if (!success)
            {
                return BadRequest("El usuario ya existe");
            }
            return Ok("Usuario registrado con éxito");
        }
        // GET /api/auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized();

            var profile = await _userService.GetProfileAsync(userId);
            return profile is null ? NotFound() : Ok(profile);
        }

        // PUT /api/auth/me
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized();

            var ok = await _userService.UpdateProfileAsync(userId, dto);
            return ok ? NoContent() : NotFound();
        }


        // PUT /api/auth/change-password
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (dto.PasswordNueva != dto.PasswordNuevaConfirm)
                return BadRequest("La confirmación no coincide.");

            var ok = await _userService.ChangePasswordAsync(userId,
                                                            dto.PasswordActual,
                                                            dto.PasswordNueva);

            return ok ? NoContent() : BadRequest("Contraseña actual incorrecta");
        }
    }

    /* ---------- MÉTODO DE EXTENSIÓN ---------- */
    internal static class ClaimsPrincipalExtensions
    {
        /// Devuelve el Guid del usuario (sub) si existe.
   
        public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
        {
            var sub = user.FindFirstValue(ClaimTypes.NameIdentifier)    // o JwtRegisteredClaimNames.Sub
                   ?? user.FindFirstValue("sub");

            return Guid.TryParse(sub, out userId);
        }
    }
}

