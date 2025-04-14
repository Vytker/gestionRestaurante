using Reservas.Application.Interfaces;
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
            if (!reservas.Any())
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
    }
}
