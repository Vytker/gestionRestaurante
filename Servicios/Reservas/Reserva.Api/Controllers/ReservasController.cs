using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservas.Application.Dtos;
using Shared.Extensions;


namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        
        private readonly IReservaService _reservaService;
        

        public ReservasController(IReservaService reservaService)
        {
            _reservaService = reservaService;
            
        }
        [HttpGet]
        [Authorize(Roles ="Owner,Staff,SuperAdmin")]
        public IActionResult GetAll()

        {
            var restId = User.RestauranteId();
            var reservas = _reservaService.ObtenerTodasReservas(restId);
            if (!reservas.Any())
            {
                return NoContent();  // Si no hay reservas
            }
            return Ok(reservas);
        }




        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservaCreateDto reservaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var restId = User.RestauranteId();
                // Llamas al servicio que ya genera y comprueba el code internamente
                var (ok,error,codigo) = await _reservaService.CrearReservaAsync(restId,reservaDto);

                if (!ok)
                    return BadRequest(new { error });

                return CreatedAtAction(nameof(GetByCode), new { code = codigo }, new { mensaje ="Reserva creada", codigo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno: " + ex.Message });
            }
        }


        [Authorize]
        [HttpPut("{id}/{estado}")]
        public IActionResult ActualizarEstado(Guid id, Reservas.Domain.Entities.Reserva.EstadoReserva nuevoEstado)
        {
            if(!Enum.IsDefined(typeof(Reservas.Domain.Entities.Reserva.EstadoReserva), nuevoEstado))
            {
                return BadRequest("Estado no válido.");
            }

            // Verifica si la reserva existe
            try
            {
                var restId = User.RestauranteId();
                _reservaService.ActualizarEstadoReserva(restId,id, nuevoEstado);
                return NoContent(); // Devuelve 204 No Content si la actualización fue exitosa
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Reserva no encontrada.");
            }
            catch (Exception ex)
            {
                // Devuelve el mensaje del error para ver qué está fallando
                return StatusCode(500, $"Error interno: {ex.Message}");
            }

        }
        // GET  /api/reservas/codigo/ABCD1234
        [HttpGet("codigo/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var restId = User.RestauranteId();
            var dto = await _reservaService.ObtenerReservaPorCodeAsync(code, restId);
            return dto == null ? NotFound() : Ok(dto);
        }

        // PUT  /api/reservas/codigo/ABCD1234
        [HttpPut("codigo/{code}")]
        public async Task<IActionResult> UpdateByCode(string code, [FromBody] ReservaUpdateDto dto)
        {
            var restId = User.RestauranteId();
            var ok = await _reservaService.ActualizarReservaPorCodeAsync(code, dto, restId);
            return ok ? NoContent() : NotFound();
        }

        // DELETE /api/reservas/codigo/ABCD1234
        [HttpDelete("codigo/{code}")]
        public async Task<IActionResult> CancelByCode(string code)
        {
            var restId = User.RestauranteId();
            var ok = await _reservaService.CancelarReservaPorCodeAsync(code, restId);
            return ok ? NoContent() : NotFound();
        }

    }
}
