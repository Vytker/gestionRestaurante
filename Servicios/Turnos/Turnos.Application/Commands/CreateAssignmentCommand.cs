using MediatR;
using Turnos.Application.Dtos;
public class CreateAssignmentCommand : IRequest<AssignmentDto>
{
    public Guid SlotId { get; }
    public DateTime Date { get; }
    public Guid EmpleadoId { get; }
    public Guid OwnerId { get; }

    public CreateAssignmentCommand(Guid slotId, DateTime date, Guid empleadoId, Guid ownerId)
    {
        SlotId = slotId; Date = date; EmpleadoId = empleadoId; OwnerId = ownerId;
    }
}
