using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnos.Infrastructure.Persistence;

public class GetAssignmentsInRangeQueryHandler
    : IRequestHandler<GetAssignmentsInRangeQuery, IEnumerable<AssignmentDto>>
{
    private readonly TurnosDbContext _context;
    public GetAssignmentsInRangeQueryHandler(TurnosDbContext ctx)
        => _context = ctx;

    public async Task<IEnumerable<AssignmentDto>> Handle(
        GetAssignmentsInRangeQuery req,
        CancellationToken ct)
    {
        // Filtramos todo aquello cuyo inicio esté dentro del rango [start, end)
        return await _context.Assignments
            .Where(a =>
            a.RestauranteId == req.RestauranteId &&
            a.FechaHoraInicio >= req.Start
                     && a.FechaHoraInicio < req.End)
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
