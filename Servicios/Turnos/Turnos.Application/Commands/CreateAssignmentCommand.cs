using MediatR;
using Turnos.Application.Common;
using Turnos.Application.Dtos;
public class CreateAssignmentCommand : IRequest<AssignmentDto>, ITenantScoped
{
    public Guid SlotId { get; }
    public DateTime Date { get; }
    public Guid EmpleadoId { get; }
    public Guid RestauranteId { get; set; }

    public CreateAssignmentCommand(Guid slotId, DateTime date, Guid empleadoId)
    {
        SlotId = slotId; Date = date; EmpleadoId = empleadoId;
    }
}
