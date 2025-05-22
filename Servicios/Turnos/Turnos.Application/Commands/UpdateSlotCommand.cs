// UpdateSlotCommand.cs
using MediatR;

public record UpdateSlotCommand(Guid SlotId, string Name, TimeSpan Start, TimeSpan End, Guid OwnerId)
    : IRequest<SlotDto>;