using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservas.Application.Dtos;
using Shared.Extensions;
using AutoQueryable.Extensions;

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

        [HttpGet("slots")]
        public async Task<IActionResult> GetSlots(
       [FromQuery] Guid restauranteId,
       [FromQuery] DateTime fecha)
        {
            var slots = await _reservaService.ObtenerSlotsDisponiblesAsync(restauranteId, fecha);
            if (!slots.Any())
                return NoContent();

            return Ok(slots);
        }

        [HttpGet]
        [Authorize(Roles ="Owner,Staff,SuperAdmin")]
        public IActionResult GetAll([FromQuery] string? query)

        {
            var restId = User.RestauranteId();
            var reservas = _reservaService.ObtenerTodasReservas(restId).AsQueryable();
            if (!reservas.Any())
            {
                return NoContent();  // Si no hay reservas
            }
            try
            {
                // Si no se pasa un query, devuelve los datos por defecto
                if (string.IsNullOrEmpty(query))
                {
                    var defaultResult = reservas
                        .OrderBy(r => r.FechaReserva) // Ordenar por fecha de reserva
                        .Take(10) // Mostrar las primeras 10 reservas
                        .ToList();

                    return Ok(new
                    {
                        Data = defaultResult,
                        Metadata = new
                        {
                            PageNumber = 1,
                            PageSize = 10,
                            TotalItemCount = reservas.Count(),
                            PageCount = (int)Math.Ceiling(reservas.Count() / 10.0),
                            HasNextPage = reservas.Count() > 10,
                            HasPreviousPage = false
                        }
                    });
                }

                // Procesar el query con AutoQueryable
                var result = reservas.AutoQueryable(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Controller] Error al procesar la consulta: {ex.Message}");
                return BadRequest(new { error = "Error al procesar la consulta.", details = ex.Message });
            }

        }




        // 2️⃣ Crear reserva (público, no autoriza)
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] Guid restauranteId, [FromBody] ReservaCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (ok, error, code) = await _reservaService.CrearReservaAsync(restauranteId, dto);
            if (!ok) return BadRequest(new { error });
            return CreatedAtAction(nameof(GetByCode), new { code, restauranteId }, new { mensaje = "Reserva creada", code });
        }


        [Authorize]
        [HttpPut("{id}/{estado}")]
        public async Task<IActionResult> ActualizarEstado(Guid id, string estado)
        {
            Console.WriteLine($"[Controller] Id de la reserva: {id}, Estado recibido como string: {estado}");
            // Intentar convertir el estado al enumerador
            if (!Enum.TryParse<Reservas.Domain.Entities.Reserva.EstadoReserva>(estado, true, out var nuevoEstado))
            {
                Console.WriteLine("[Controller] Estado no válido.");
                return BadRequest("Estado no válido.");
            }
            Console.WriteLine($"[Controller] Estado convertido al enumerador: {nuevoEstado}");

            // Verifica si la reserva existe
            try
            {
                var restId = User.RestauranteId();
                Console.WriteLine($"[Controller] ID del restaurante: {restId}");
                await _reservaService.ActualizarEstadoReserva(id, restId, nuevoEstado);
                Console.WriteLine("[Controller] Estado actualizado correctamente.");
                return NoContent();
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
        public async Task<IActionResult> GetByCode(
           [FromQuery] Guid restauranteId,
           string code)
        {
            var dto = await _reservaService.ObtenerReservaPorCodeAsync(code, restauranteId);
            return dto is null ? NotFound() : Ok(dto);
        }
        // PUT  /api/reservas/codigo/ABCD1234
        [HttpPut("codigo/{code}")]
        public async Task<IActionResult> UpdateByCode(
            [FromQuery] Guid restauranteId,
            string code,
            [FromBody] ReservaUpdateDto dto)
        {
            var ok = await _reservaService.ActualizarReservaPorCodeAsync(code, dto, restauranteId);
            return ok ? NoContent() : NotFound();
        }

        // DELETE /api/reservas/codigo/ABCD1234
        [HttpDelete("codigo/{code}")]
        public async Task<IActionResult> CancelByCode(
            [FromQuery] Guid restauranteId,
            [FromRoute]string code)
        {
            var ok = await _reservaService.CancelarReservaPorCodeAsync(code, restauranteId);
            return ok ? NoContent() : NotFound();
        }

    }
}
