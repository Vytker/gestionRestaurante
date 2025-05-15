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

    public async Task<IEnumerable<AssignmentDto>> Handle(GetAssignmentsByDateQuery req, CancellationToken ct)
        => await _context.Assignments
            .Where(a => a.Date == req.Date)
            .Select(a => new AssignmentDto
            {
                Id = a.Id,
                SlotId = a.SlotId,
                Date = a.Date,
                EmpleadoId = a.EmpleadoId
            })
            .ToListAsync(ct);
}