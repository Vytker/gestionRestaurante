using MediatR;
using System;
using Turnos.Application.Dtos;

namespace Turnos.Application.Commands
{
    // Devuelve el ShiftDto recién creado
    public class CreateShiftCommand : IRequest<ShiftDto>
    {
        public Guid EmployeeId { get; }
        public TimeSpan Start { get; }
        public TimeSpan End { get; }
        public Guid OwnerId { get; }

        public CreateShiftCommand(Guid employeeId, TimeSpan start, TimeSpan end, Guid ownerId)
        {
            EmployeeId = employeeId;
            Start = start;
            End = end;
            OwnerId = ownerId;
        }
    }
}
