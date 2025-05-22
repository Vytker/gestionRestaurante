using MediatR;

public class DeleteSlotCommand : IRequest<Unit>
{
    public Guid SlotId { get; }
    public Guid OwnerId { get; }
    public DeleteSlotCommand(Guid slotId, Guid ownerId)
    {
        SlotId = slotId;
        OwnerId = ownerId;
    }
}