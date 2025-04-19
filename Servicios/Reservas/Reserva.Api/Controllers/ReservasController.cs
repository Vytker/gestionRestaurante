using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



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
        public IActionResult GetAll()
        {
            var reservas = _reservaService.ObtenerTodasReservas();
            if (!reservas.Any() || reservas == null)
            {
                return NoContent();  // Si no hay reservas
            }
            return Ok(reservas);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Reservas.Domain.Entities.Reserva reserva)
        {
            try
            {
                _reservaService.CrearReserva(reserva);
                return CreatedAtAction(nameof(GetAll), new { id = reserva.Id }, reserva);
            }
            catch (Exception ex)
            {
                // Devuelve el mensaje del error para ver qué está fallando
                return StatusCode(500, $"Error interno: {ex.Message}");
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
                _reservaService.ActualizarEstadoReserva(id, nuevoEstado);
                return NoContent(); // Devuelve 204 No Content si la actualización fue exitosa
            }
            catch (Exception ex)
            {
                // Devuelve el mensaje del error para ver qué está fallando
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
