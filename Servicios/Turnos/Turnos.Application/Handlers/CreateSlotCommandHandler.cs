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

        

        Guid ownerId;
        if (req.IsSuperAdmin)
        {
            // el superadmin debe indicar el propietario explícitamente
            if (req.OwnerId is null)
            throw new InvalidOperationException("Debe indicar OwnerId.");
        
            ownerId = req.OwnerId.Value;
        }
        else
        {
            // lo creó un Owner → viene su Id
            ownerId = req.OwnerId!.Value;
        }
        var horario = new IntervaloTiempo(req.Start, req.End);
        var slot = new Slot(req.Name, horario, ownerId);

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