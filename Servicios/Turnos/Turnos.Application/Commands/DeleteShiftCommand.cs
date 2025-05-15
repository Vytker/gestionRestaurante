
using MediatR;
using System;

namespace Turnos.Application.Commands
{
    public class DeleteShiftCommand : IRequest<Unit>
    {
        public Guid ShiftId { get; }
        public Guid OwnerId { get; }

        public DeleteShiftCommand(Guid shiftId, Guid ownerId)
        {
            ShiftId = shiftId;
            OwnerId = ownerId;
        }
    }
}
