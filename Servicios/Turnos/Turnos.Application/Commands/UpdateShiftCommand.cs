using MediatR;
using System;
using Turnos.Application.Common;
using Turnos.Application.Dtos;

namespace Turnos.Application.Commands
{
    public class UpdateShiftCommand : IRequest<Unit>, ITenantScoped
    {
        public Guid ShiftId { get; }
        public TimeSpan Start { get; }
        public TimeSpan End { get; }
        public Guid RestauranteId { get; set; }

        public UpdateShiftCommand(Guid shiftId, TimeSpan start, TimeSpan end)
        {
            ShiftId = shiftId;
            Start = start;
            End = end;
            
        }
    }
}
