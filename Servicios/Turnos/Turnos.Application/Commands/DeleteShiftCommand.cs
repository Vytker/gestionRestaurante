
using MediatR;
using System;
using Turnos.Application.Common;

namespace Turnos.Application.Commands
{
    public class DeleteShiftCommand : IRequest<Unit>, ITenantScoped
    {
        public Guid ShiftId { get; }
        public Guid RestauranteId { get; set; }

        public DeleteShiftCommand(Guid shiftId)
        {
            ShiftId = shiftId;
            
        }
    }
}
