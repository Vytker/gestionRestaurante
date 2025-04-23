
using Microsoft.AspNetCore.Mvc;
using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
    }
}
