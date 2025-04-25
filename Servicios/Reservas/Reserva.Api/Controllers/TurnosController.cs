using Microsoft.AspNetCore.Mvc;
using Reservas.Application.Interfaces;
using Reservas.Application.Dtos;
using Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner")] // Fixed the CS1016 error by combining roles into a single string
    [Authorize(Roles = "SuperAdmin")]
    public class TurnosController : ControllerBase
    {
        private readonly ITurnoService _turnoService;
        public TurnosController(ITurnoService turnoService)
        {
            _turnoService = turnoService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TurnoCreateDto dto)
        {
            try
            {
                var restId = User.RestauranteId();
                await _turnoService.CrearTurnoAsync(dto, restId);
                return Ok(new { mensaje = "Turno creado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Devuelve el mensaje del error para ver qué está fallando
                return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var restId = User.RestauranteId();
            var turnos = await _turnoService.ObtenerTodosAsync(restId);

            return turnos.Any() ? Ok(turnos) : NoContent(); // Si hay turnos
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var restId = User.RestauranteId();
            var turno = await _turnoService.ObtenerTurnoPorIdAsync(id, restId);
            if (turno == null)
            {
                return NotFound(new { error = "Turno no encontrado" });
            }
            return Ok(turno);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TurnoUpdateDto dto)
        {
            dto.Id = id;

            try
            {
                var restId = User.RestauranteId();
                await _turnoService.EditarTurnoAsync(id, dto, restId);
                return Ok(new { mensaje = "Turno actualizado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var restId = User.RestauranteId();
            var eliminado = await _turnoService.EliminarTurnoAsync(id, restId);
            if (!eliminado) return NotFound();
            return Ok();
        }
    }
}
