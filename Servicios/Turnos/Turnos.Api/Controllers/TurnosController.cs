using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Turnos.Application.Commands;
using Turnos.Application.Dtos;
using Turnos.Application.Queries;

namespace Turnos.Api.Controllers
{
    [ApiController]
    [Route("api/v1/gestion-horarios")]
    public class GestionHorariosController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GestionHorariosController(IMediator mediator)
            => _mediator = mediator;

        // ─── Slots ──────────────────────────────────────
        [HttpGet("slots"), Authorize]
        public async Task<IActionResult> GetAllSlots()
        {
            var slots = await _mediator.Send(new GetAllSlotsQuery());
            return Ok(slots);
        }
            

        [HttpPost("slots"), Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateSlot([FromBody] CreateSlotDto dto)
        {
            var ownerIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (ownerIdClaim == null)
                return Forbid("Missing sub claim.");

            var ownerId = Guid.Parse(ownerIdClaim.Value);

            var cmd = new CreateSlotCommand(dto.Name, dto.Start, dto.End, ownerId);
            var slot = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetAllSlots), new { id = slot.Id }, slot);
        }

        // ─── Assignments ───────────────────────────────
        [HttpGet("assignments"), Authorize]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var list = await _mediator.Send(new GetAssignmentsByDateQuery(date));
            return Ok(list);
        }

        [HttpPost("assignments"), Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDto dto)
        {
            var ownerIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (ownerIdClaim == null)
                return Forbid("Missing sub claim.");

            var ownerId = Guid.Parse(ownerIdClaim.Value);

            var cmd = new CreateAssignmentCommand(dto.SlotId, dto.Date, dto.EmpleadoId, ownerId);
            var asg = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetByDate), new { date = dto.Date }, asg);
        }

        [HttpDelete("assignments/{id}"), Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteAssignment(Guid id)
        {
            await _mediator.Send(new DeleteAssignmentCommand(id));
            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? day)
        {
            var query = new GetAllShiftsQuery(day);
            var shifts = await _mediator.Send(query);
            return Ok(shifts);
        }

        [HttpGet("empleados/{empleadoId}")]
        [Authorize]
        public async Task<IActionResult> GetByEmployee(
            Guid empleadoId,
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta)
        {
            var query = new GetShiftsByEmployeeQuery(empleadoId, desde, hasta);
            var shifts = await _mediator.Send(query);
            return Ok(shifts);
        }

        /// <summary>
        /// POST /api/v1/gestion-horarios
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create([FromBody] CreateShiftDto dto)
        {
            var ownerId = Guid.Parse(User.FindFirst("sub").Value);
            var command = new CreateShiftCommand(dto.EmployeeId, dto.Start, dto.End, ownerId);
            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetByEmployee),
                new
                {
                    empleadoId = result.EmpleadoId,
                    desde = result.FechaHoraInicio,
                    hasta = result.FechaHoraFin
                },
                result);
        }
    }
}
