using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Turnos.Application.Dtos;
using Turnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class GetAllSlotsQueryHandler : IRequestHandler<GetAllSlotsQuery, IEnumerable<SlotDto>>
{
    private readonly TurnosDbContext _context;
    public GetAllSlotsQueryHandler(TurnosDbContext ctx) => _context = ctx;

    public async Task<IEnumerable<SlotDto>> Handle(GetAllSlotsQuery req, CancellationToken ct)
        => await _context.Slots
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.RestauranteId == req.RestauranteId)
            .Select(s => new SlotDto
            {
                Id = s.Id,
                Name = s.Name,
                Start = s.Horario.Inicio,
                End = s.Horario.Fin
            })
            .ToListAsync(ct);
}
