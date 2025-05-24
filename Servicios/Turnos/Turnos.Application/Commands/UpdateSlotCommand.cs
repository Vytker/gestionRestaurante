using MediatR;
using Turnos.Application.Common;

public record UpdateSlotCommand(
        Guid SlotId,
        string Name,
        TimeSpan Start,
        TimeSpan End)
    : IRequest<SlotDto>, ITenantScoped
{
    // El pipeline asigna este valor antes de llegar al handler
    public Guid RestauranteId { get; set; }
}
