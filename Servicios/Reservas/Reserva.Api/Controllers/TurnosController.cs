using Microsoft.AspNetCore.Mvc;
using Reservas.Application.Interfaces;
using Reservas.Application.Dtos;

namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                await _turnoService.CrearTurnoAsync(dto);
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
            var turnos = await _turnoService.ObtenerTodosTurnosAsync();
            if (turnos == null || !turnos.Any())
            {
                return NoContent(); // Si no hay turnos
            }
            return Ok(turnos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var turno = await _turnoService.ObtenerTurnoPorIdAsync(id);
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
                await _turnoService.EditarTurnoAsync(id,dto);
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
            var eliminado = await _turnoService.EliminarTurnoAsync(id);
            if (!eliminado) return NotFound();
            return Ok();
        }
    }
}
