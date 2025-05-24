using MediatR;
using Turnos.Application.Common;
using Turnos.Application.Dtos;

public record GetAllSlotsQuery : IRequest<IEnumerable<SlotDto>>, ITenantScoped
{
    // El TenantResolutionBehavior rellenará esto
    public Guid RestauranteId { get; set; }
}