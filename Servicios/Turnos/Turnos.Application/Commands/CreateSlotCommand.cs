
using MediatR;
using Turnos.Application.Dtos;
public class CreateSlotCommand : IRequest<SlotDto>
{
    public string Name { get; }
    public TimeSpan Start { get; }
    public TimeSpan End { get; }
    public Guid OwnerId { get; }

    public CreateSlotCommand(string name, TimeSpan start, TimeSpan end, Guid ownerId)
    {
        Name = name; Start = start; End = end; OwnerId = ownerId;
    }
}