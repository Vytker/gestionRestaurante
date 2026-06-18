using Microsoft.AspNetCore.Mvc;
using Reservas.Application.Interfaces;
using Reservas.Application.Dtos;
using Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet]// GET /api/turnos?restauranteId=....
        [Authorize(Roles = "Owner,SuperAdmin")]
        public async Task<IActionResult> GetAll([FromQuery] Guid? restauranteId = null)
        {
            Guid? restId;
            if (User.IsInRole("SuperAdmin"))
            {
                if (restauranteId == null || restauranteId == Guid.Empty)
                    return BadRequest(new { error = "Debe indicar el restaurante." });

                restId = restauranteId;
            }
            else
            {
                restId = User.RestauranteId();
                if (restauranteId != null && restauranteId != restId)
                    return Forbid();    
            }
            var turnos = await _turnoService.ObtenerTurnosAsync(restId);
            return turnos.Any() ? Ok(turnos) : NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Owner,SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] TurnoCreateDto dto)
        {

            Guid restauranteId;

            if (User.IsInRole("SuperAdmin"))
            {
                if (dto.RestauranteId == Guid.Empty)
                    return BadRequest(new { error = "Debe indicar el restaurante." });
                restauranteId = dto.RestauranteId;
            }
            else
            {
                restauranteId = User.RestauranteId();
                if (dto.RestauranteId != Guid.Empty && dto.RestauranteId != restauranteId)
                    return Forbid();
            }

            try
            {
                
                await _turnoService.CrearTurnoAsync(dto, restauranteId);
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
                return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromQuery] Guid restauranteId)
        {
            var turno = await _turnoService.ObtenerTurnoPorIdAsync(id, restauranteId);
            return turno is null
                 ? NotFound(new { error = "Turno no encontrado" })
                 : Ok(turno);
        }

        /// GET /api/turnos/slots?restauranteId={id}&fecha={yyyy-MM-dd}
        [HttpGet("slots")]
        public async Task<IActionResult> GetSlots(
            [FromQuery] Guid restauranteId,
            [FromQuery] DateTime fecha)
        {
            var slots = await _turnoService.ObtenerSlotsDisponiblesAsync(restauranteId, fecha);
            return slots.Any() ? Ok(slots) : NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner,SuperAdmin")]
        
        public async Task<IActionResult> Update(int id, [FromBody] TurnoUpdateDto dto, [FromQuery] Guid? restauranteId = null)
        {

            Guid restId;
            if (User.IsInRole("SuperAdmin"))
            {
                if (restauranteId == null || restauranteId == Guid.Empty)
                    return BadRequest(new { error = "Debe indicar el restaurante." });
                restId = restauranteId.Value;
            }
            else
            {
                restId = User.RestauranteId();
                if (restauranteId != null && restauranteId != restId)
                    return Forbid();
            }

            dto.Id = id;

            try
            {
                
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
        [Authorize(Roles = "Owner,SuperAdmin")]
        public async Task<IActionResult> Delete(int id, [FromQuery] Guid? restauranteId = null)
        {

            Guid restId;
            if (User.IsInRole("SuperAdmin"))
            {
                if (restauranteId == null || restauranteId == Guid.Empty)
                    return BadRequest(new { error = "Debe indicar el restaurante." });
                restId = restauranteId.Value;
            }
            else
            {
                restId = User.RestauranteId();
                if (restauranteId != null && restauranteId != restId)
                    return Forbid();
            }
            var eliminado = await _turnoService.EliminarTurnoAsync(id, restId);
            if (!eliminado) return NotFound();
            return Ok();
        }
    }
}
