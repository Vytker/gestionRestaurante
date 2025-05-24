// Turnos.Application/Handlers/GetSlotByIdQueryHandler.cs
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnos.Application.Dtos;
using Turnos.Application.Queries;
using Turnos.Infrastructure.Persistence;

namespace Turnos.Application.Handlers
{
    public class GetSlotByIdQueryHandler
        : IRequestHandler<GetSlotByIdQuery, SlotDto?>
    {
        private readonly TurnosDbContext _ctx;
        public GetSlotByIdQueryHandler(TurnosDbContext ctx)
            => _ctx = ctx;

        public async Task<SlotDto?> Handle(
            GetSlotByIdQuery req,
            CancellationToken ct)
        {
            return await _ctx.Slots
                .Where(s =>
                s.RestauranteId == req.RestauranteId &&
                s.Id == req.SlotId && !s.IsDeleted)
                .Select(s => new SlotDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Start = s.Horario.Inicio,
                    End = s.Horario.Fin,
                    IsDeleted = s.IsDeleted
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
