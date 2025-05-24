
using MediatR;
using Turnos.Application.Common;
using Turnos.Application.Dtos;
public class CreateSlotCommand : IRequest<SlotDto>, ITenantScoped
{
    public string Name { get; }
    public TimeSpan Start { get; }
    public TimeSpan End { get; }
    public Guid RestauranteId { get; set; }

    public CreateSlotCommand(string name, TimeSpan start, TimeSpan end)
    {
        Name = name; Start = start; End = end;
    }
}