// Turnos.Application/Handlers/GetAllShiftsQueryHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Turnos.Application.Queries;
using Turnos.Application.Dtos;
using Turnos.Infrastructure.Persistence;

namespace Turnos.Application.Handlers
{
    public class GetAllShiftsQueryHandler
        : IRequestHandler<GetAllShiftsQuery, IEnumerable<ShiftDto>>
    {
        private readonly TurnosDbContext _context;

        public GetAllShiftsQueryHandler(TurnosDbContext context)
            => _context = context;

        public async Task<IEnumerable<ShiftDto>> Handle(
            GetAllShiftsQuery request,
            CancellationToken cancellationToken)
        {
            // Base query
            var query = _context.Turnos.AsQueryable();

            if (request.Day.HasValue)
            {
                var startOfDay = request.Day.Value.Date;
                var endOfDay = startOfDay.AddDays(1);

                var startTime = startOfDay.TimeOfDay;
                var endTime = endOfDay.TimeOfDay;

                query = query.Where(t =>
                    t.Horario.Inicio >= startTime &&
                    t.Horario.Inicio < endTime
                );
            }

            var list = await query
                .Select(t => new ShiftDto
                {
                    Id = t.Id,
                    EmpleadoId = t.EmpleadoId,
                    FechaHoraInicio = t.Horario.Inicio,
                    FechaHoraFin = t.Horario.Fin
                })
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}
