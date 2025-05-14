using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Turnos.Application.Dtos;
using Turnos.Application.Queries;

namespace Turnos.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TurnosController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TurnosController(IMediator mediator) => _mediator = mediator;

        [HttpGet("empleados/{empleadoId}")]
        [Authorize]
        public async Task<IActionResult> GetByEmployee(Guid empleadoId, [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        {
            var query = new GetShiftsByEmployeeQuery(empleadoId, desde, hasta);
            var shifts = await _mediator.Send(query);
            return Ok(shifts);
        }
    }
}
