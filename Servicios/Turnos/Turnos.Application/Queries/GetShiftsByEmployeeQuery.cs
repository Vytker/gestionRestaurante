using MediatR;
using System;
using System.Collections.Generic;
using Turnos.Application.Dtos;

namespace Turnos.Application.Queries
{
    public class GetShiftsByEmployeeQuery : IRequest<IEnumerable<ShiftDto>>
    {
        public Guid EmpleadoId { get; }
        public DateTime Desde { get; }
        public DateTime Hasta { get; }

        public GetShiftsByEmployeeQuery(Guid empleadoId, DateTime desde, DateTime hasta)
        {
            EmpleadoId = empleadoId;
            Desde = desde;
            Hasta = hasta;
        }
    }
}
