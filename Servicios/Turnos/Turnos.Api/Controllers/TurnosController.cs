using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        // ─── Slots
        [HttpGet("slots"), Authorize]
        public async Task<IActionResult> GetAllSlots()
        {
            var slots = await _mediator.Send(new GetAllSlotsQuery());
            return Ok(slots);
        }
            

        [HttpPost("slots"), Authorize(Roles = "Owner,SuperAdmin")]
        public async Task<IActionResult> CreateSlot([FromBody] CreateSlotDto dto)
        {
            var isSuper = User.IsInRole("SuperAdmin");
            Guid? ownerId = isSuper ? null
                                    : Guid.Parse(User.FindFirst("sub").Value);

            var cmd = new CreateSlotCommand(dto.Name, dto.Start, dto.End, ownerId, isSuper);
            var slot = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetAllSlots), new { id = slot.Id }, slot);
        }

        
        [HttpPut("slots/{id}")]
        [Authorize(Roles = "Owner,SuperAdmin")]
        public async Task<IActionResult> UpdateSlot(
            Guid id,
            [FromBody] UpdateSlotDto dto)
        {
            var ownerIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (ownerIdClaim == null)
                return Forbid("Missing sub claim.");

            var ownerId = Guid.Parse(ownerIdClaim.Value);

            var updated = await _mediator.Send(
                new UpdateSlotCommand(id, dto.Name, dto.Start, dto.End, ownerId)
            );
            return Ok(updated);
        }

        // GET /api/v1/gestion-horarios/slots/{id}
        [HttpGet("slots/{id}"), Authorize]
        public async Task<IActionResult> GetSlot(Guid id)
        {
            var dto = await _mediator.Send(new GetSlotByIdQuery(id));
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpDelete("slots/{id}")]
        [Authorize(Roles = "Owner,SuperAdmin")]
        public async Task<IActionResult> DeleteSlot(Guid id)
        {
            var ownerIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (ownerIdClaim == null)
                return Forbid("Missing sub claim.");

            var ownerId = Guid.Parse(ownerIdClaim.Value);

            await _mediator.Send(new DeleteSlotCommand(id, ownerId));
            return NoContent();
        }

        // ─── Assignments
        [HttpGet("assignments"), Authorize]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var list = await _mediator.Send(new GetAssignmentsByDateQuery(date));
            return Ok(list);
        }

        // Despues rango completo:
        [HttpGet("assignments/range"), Authorize]
        public async Task<IActionResult> GetInRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var list = await _mediator.Send(new GetAssignmentsInRangeQuery(start, end));
            return Ok(list);
        }

        [HttpPost("assignments"), Authorize(Roles = "Owner,SuperAdmin")]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDto dto)
        {
            var isSuper = User.IsInRole("SuperAdmin");
            Guid? ownerId = null;
            if (!isSuper)                   // sólo los Owners necesitan OwnerId
                ownerId = Guid.Parse(User.FindFirst("sub").Value);

            var cmd = new CreateAssignmentCommand(dto.SlotId, dto.Date, dto.EmpleadoId, ownerId, isSuper);
            var asg = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetByDate), new { date = dto.Date }, asg);
        }

        [HttpDelete("assignments/{id}"), Authorize(Roles = "Owner,SuperAdmin")]
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

        
        [HttpPost]
        [Authorize(Roles = "Owner,SuperAdmin")]
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
