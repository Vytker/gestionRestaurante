using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnos.Infrastructure.Persistence;

public class UpdateSlotCommandHandler : IRequestHandler<UpdateSlotCommand, SlotDto>
{
    private readonly TurnosDbContext _ctx;
    public UpdateSlotCommandHandler(TurnosDbContext ctx) => _ctx = ctx;

    public async Task<SlotDto> Handle(UpdateSlotCommand req, CancellationToken ct)
    {
        var slot = await _ctx.Slots
      .FirstOrDefaultAsync(s =>
          s.Id == req.SlotId &&
          s.RestauranteId == req.RestauranteId &&  // filtro tenant
          !s.IsDeleted,
          ct)
      ?? throw new KeyNotFoundException("Slot no encontrado en este restaurante");

        // (Opcional) validar que OwnerId coincide con slot.OwnerId...

        slot.Update(
            req.Name,
            new Turnos.Domain.ValueObjects.IntervaloTiempo(req.Start, req.End));
        await _ctx.SaveChangesAsync(ct);

        return new SlotDto
        {
            Id = slot.Id,
            Name = slot.Name,
            Start = slot.Horario.Inicio,
            End = slot.Horario.Fin,
            IsDeleted = slot.IsDeleted
        };
    }
}