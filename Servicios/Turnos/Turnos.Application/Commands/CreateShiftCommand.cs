using MediatR;
using System;
using Turnos.Application.Common;
using Turnos.Application.Dtos;

namespace Turnos.Application.Commands
{
    // Devuelve el ShiftDto recién creado
    public class CreateShiftCommand : IRequest<ShiftDto>, ITenantScoped
    {
        public Guid EmployeeId { get; }
        public TimeSpan Start { get; }
        public TimeSpan End { get; }
        public Guid RestauranteId { get; set; }

        public CreateShiftCommand(Guid employeeId, TimeSpan start, TimeSpan end)
        {
            EmployeeId = employeeId;
            Start = start;
            End = end;
            
        }
    }
}
