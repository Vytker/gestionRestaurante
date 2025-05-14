using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Turnos.Application.Dtos;
using Turnos.Application.Queries;
using Turnos.Infrastructure.Persistence;

namespace Turnos.Application.Handlers
{
    public class GetShiftsByEmployeeQueryHandler : IRequestHandler<GetShiftsByEmployeeQuery, IEnumerable<ShiftDto>>
    {
        private readonly TurnosDbContext _context;
        public GetShiftsByEmployeeQueryHandler(TurnosDbContext context) => _context = context;

        public async Task<IEnumerable<ShiftDto>> Handle(GetShiftsByEmployeeQuery request, CancellationToken cancellationToken)
        {
            var shifts = await _context.Turnos
                .AsNoTracking()
                .Where(t => t.EmpleadoId == request.EmpleadoId
                         && t.Horario.Inicio >= request.Desde
                         && t.Horario.Fin <= request.Hasta)
                .OrderBy(t => t.Horario.Inicio)
                .Select(t => new ShiftDto
                {
                    Id = t.Id,
                    EmpleadoId = t.EmpleadoId,
                    FechaHoraInicio = t.Horario.Inicio,
                    FechaHoraFin = t.Horario.Fin
                })
                .ToListAsync(cancellationToken);

            return shifts;
        }
    }
}
