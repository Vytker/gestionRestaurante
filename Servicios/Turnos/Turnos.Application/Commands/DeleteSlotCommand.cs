using MediatR;
using Turnos.Application.Common;

public class DeleteSlotCommand : IRequest<Unit>, ITenantScoped
{
    public Guid SlotId { get; }
    public Guid RestauranteId { get; set; }
    public DeleteSlotCommand(Guid slotId)
    {
        SlotId = slotId;
        
    }
}