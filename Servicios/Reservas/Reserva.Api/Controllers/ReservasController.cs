
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Reservas.Application.Dtos;
using Shared.Extensions;



namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/reservas")]
    public class ReservasController : ControllerBase
    {
        
        private readonly IReservaService _reservaService;
        

        public ReservasController(IReservaService reservaService)
        {
            _reservaService = reservaService;
            
        }

        [HttpGet("/api/reservas/slots", Order = -1)]
        public async Task<IActionResult> GetSlots(
       [FromQuery] Guid restauranteId,
       [FromQuery] DateTime fecha)
        {
            var slots = await _reservaService.ObtenerSlotsDisponiblesAsync(restauranteId, fecha);
            if (!slots.Any())
                return NoContent();

            return Ok(slots);
        }


    
        [HttpGet("/api/odata/Reservas")]
        [EnableQuery]         
        [Authorize(Roles = "Owner,Staff,SuperAdmin")]
        public IQueryable GetAll([FromQuery] Guid? restauranteId = null)
        {
            //SuperAdmin
            if (User.IsInRole("SuperAdmin"))
            {
                //  - Sin parámetro: devuelve TODAS las reservas
                if (restauranteId is null)
                    return _reservaService.ObtenerTodas();

                //  - Con parámetro: filtra por el restaurante indicado
                return _reservaService.ObtenerTodasReservas(restauranteId.Value);
            }

            // ─Owner / Staff
            //restringidos a su propio restaurante:
            var restIdClaim = User.RestauranteId();
            return _reservaService.ObtenerTodasReservas(restIdClaim);
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


        [Authorize (Roles = "Owner,SuperAdmin")]
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

                if(User.IsInRole("SuperAdmin"))
                {
                    await _reservaService.ActualizarEstadoReservaSuperAdminAsync(id, nuevoEstado);
                    Console.WriteLine("[Controller] Estado actualizado correctamente por SuperAdmin.");
                }
                else
                {
                    var restId = User.RestauranteId();
                    // Si no es SuperAdmin, solo actualiza el estado
                    await _reservaService.ActualizarEstadoReserva(id, restId, nuevoEstado);
                    Console.WriteLine("[Controller] Estado actualizado correctamente por Owner/Staff.");
                }
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



        [Authorize(Roles = "Owner,Staff,SuperAdmin")]
        [HttpGet("total")]
        public async Task<IActionResult> GetTotal(
    [FromQuery] Guid restauranteId,
    [FromQuery] DateTime fechaDesde,
    [FromQuery] DateTime fechaHasta,
    [FromQuery] string? estado = null       // ← nuevo parámetro opcional
)
        {
            // validaciones fechaDesde<=fechaHasta, etc. si lo deseas

            var total = await _reservaService.ContarReservasAsync(
                restauranteId, fechaDesde, fechaHasta, estado);

            return Ok(new { total });
        }

        [Authorize(Roles = "Owner,Staff,SuperAdmin")]
        [HttpGet("series")]
        public async Task<IActionResult> GetSeries(
            [FromQuery] Guid restauranteId,
            [FromQuery] DateTime fechaDesde,
            [FromQuery] DateTime fechaHasta,
            [FromQuery] string? estado = null       // ← nuevo parámetro opcional
        )
        {
            var series = await _reservaService.ObtenerSeriesDiariasAsync(
                restauranteId, fechaDesde, fechaHasta, estado);

            var result = series
              .Select(x => new {
                  fecha = x.Fecha.ToString("yyyy-MM-dd"),
                  total = x.Total
              });

            return Ok(result);
        }

        [Authorize(Roles = "Owner,Staff,SuperAdmin")]
        [HttpGet("series/hourly")]
        public async Task<IActionResult> GetHourlySeries(
    [FromQuery] Guid restauranteId,
    [FromQuery] DateTime fecha,
    [FromQuery] string? estado = null)
        {
            var horas = await _reservaService.ObtenerSeriesHorariasAsync(restauranteId, fecha, estado);
            // Ya vienen en el formato { Hour, Total }
            return Ok(horas);
        }
    }
}
