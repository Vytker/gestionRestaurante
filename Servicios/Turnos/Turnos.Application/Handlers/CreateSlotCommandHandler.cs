using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Turnos.Domain.Entities;
using Turnos.Domain.ValueObjects;
using Turnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CreateSlotCommandHandler : IRequestHandler<CreateSlotCommand, SlotDto>
{
    private readonly TurnosDbContext _context;
    public CreateSlotCommandHandler(TurnosDbContext ctx) => _context = ctx;

    public async Task<SlotDto> Handle(CreateSlotCommand req, CancellationToken ct)
    {
        var horario = new IntervaloTiempo(req.Start, req.End);
        var slot = new Slot(req.Name, horario, req.OwnerId);

        _context.Slots.Add(slot);
        await _context.SaveChangesAsync(ct);

        return new SlotDto
        {
            Id = slot.Id,
            Name = slot.Name,
            Start = slot.Horario.Inicio,
            End = slot.Horario.Fin
        };
    }
}