using MediatR;
using System;
using Turnos.Application.Dtos;

namespace Turnos.Application.Commands
{
    public class UpdateShiftCommand : IRequest<Unit>
    {
        public Guid ShiftId { get; }
        public TimeSpan Start { get; }
        public TimeSpan End { get; }
        public Guid OwnerId { get; }

        public UpdateShiftCommand(Guid shiftId, TimeSpan start, TimeSpan end, Guid ownerId)
        {
            ShiftId = shiftId;
            Start = start;
            End = end;
            OwnerId = ownerId;
        }
    }
}
