using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Turnos.Application.Dtos;
using Turnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class GetAssignmentsByDateQueryHandler : IRequestHandler<GetAssignmentsByDateQuery, IEnumerable<AssignmentDto>>
{
    private readonly TurnosDbContext _context;
    public GetAssignmentsByDateQueryHandler(TurnosDbContext ctx) => _context = ctx;

    public async Task<IEnumerable<AssignmentDto>> Handle(
       GetAssignmentsByDateQuery req,
       CancellationToken ct)
    {
        var from = req.Date.Date;
        var to = from.AddDays(1);

        return await _context.Assignments
            // todas las asignaciones cuyo inicio cae en [from, to)
            .Where(a =>
            a.RestauranteId == req.RestauranteId &&
            a.FechaHoraInicio >= from
                     && a.FechaHoraInicio < to)
            .Select(a => new AssignmentDto
            {
                Id = a.Id,
                SlotId = a.SlotId,
                FechaHoraInicio = a.FechaHoraInicio,
                FechaHoraFin = a.FechaHoraFin,
                EmpleadoId = a.EmpleadoId
            })
            .ToListAsync(ct);
    }
}