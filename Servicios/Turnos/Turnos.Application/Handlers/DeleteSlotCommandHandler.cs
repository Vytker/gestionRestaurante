// DeleteSlotCommandHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnos.Infrastructure.Persistence;

public class DeleteSlotCommandHandler : IRequestHandler<DeleteSlotCommand, Unit>
{
    private readonly TurnosDbContext _ctx;
    public DeleteSlotCommandHandler(TurnosDbContext ctx) => _ctx = ctx;

    public async Task<Unit> Handle(DeleteSlotCommand req, CancellationToken ct)
    {
        var slot = await _ctx.Slots
     .FirstOrDefaultAsync(s =>
         s.Id == req.SlotId &&
         s.RestauranteId == req.RestauranteId &&  // ← filtro tenant
         !s.IsDeleted,
         ct)
     ?? throw new KeyNotFoundException("Slot no encontrado en este restaurante");

        // marcar soft-delete
        slot.IsDeleted = true;
        await _ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}