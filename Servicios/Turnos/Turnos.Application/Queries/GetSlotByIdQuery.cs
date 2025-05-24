using System;
using MediatR;
using Turnos.Application.Common;
using Turnos.Application.Dtos;

public record GetSlotByIdQuery(Guid SlotId)
    : IRequest<SlotDto?>, ITenantScoped
{
    // Inyectado por TenantResolutionBehavior
    public Guid RestauranteId { get; set; }
}