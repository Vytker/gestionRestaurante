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

        //comprobar que no exista un slot con el mismo nombre
        var existe = await _context.Slots
                       .AnyAsync(s => s.Name == req.Name &&
                                      s.RestauranteId == req.RestauranteId,
                                 ct);
        if (existe)
            throw new InvalidOperationException("Ya existe un slot con ese nombre en este restaurante");

        var horario = new IntervaloTiempo(req.Start, req.End);
        var slot = new Slot(req.Name, horario, req.RestauranteId);

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