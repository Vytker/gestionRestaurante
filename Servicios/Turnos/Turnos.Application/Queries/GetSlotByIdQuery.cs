
using System;
using MediatR;
using Turnos.Application.Dtos;

namespace Turnos.Application.Queries
{
    public record GetSlotByIdQuery(Guid SlotId) : IRequest<SlotDto?>;
}