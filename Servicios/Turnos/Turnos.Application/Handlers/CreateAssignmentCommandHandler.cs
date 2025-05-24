using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnos.Domain.Entities;
using Turnos.Infrastructure.Persistence;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, AssignmentDto>
{
    private readonly TurnosDbContext _context;
    public CreateAssignmentCommandHandler(TurnosDbContext ctx) => _context = ctx;

    public async Task<AssignmentDto> Handle(CreateAssignmentCommand req, CancellationToken ct)
    {
        // 1) obtengo el slot para leer sus TimeSpans
        var slot = await _context.Slots
              .SingleOrDefaultAsync(s =>
                    s.Id == req.SlotId &&
                    s.RestauranteId == req.RestauranteId, ct)
              ?? throw new UnauthorizedAccessException(
                       "Slot no pertenece a este restaurante");

        var fechaSolo = req.Date.Date;
        var tsInicio = slot.Horario.Inicio;
        var tsFin = slot.Horario.Fin;

        var dtInicio = fechaSolo + tsInicio;
        var dtFin = fechaSolo + tsFin;

        // si el fin es <= al inicio, le sumo un día
        if (tsFin <= tsInicio)
        {
            dtFin = fechaSolo.AddDays(1) + tsFin;
        }

        // 2) construir correctamente el Assignment
        var asg = new Assignment(
            slotId: slot.Id,
            fechaHoraInicio: dtInicio,
            fechaHoraFin: dtFin,
            empleadoId: req.EmpleadoId,
            restauranteId: req.RestauranteId
        );

        _context.Assignments.Add(asg);
        await _context.SaveChangesAsync(ct);

        // 3) devolver DTO
        return new AssignmentDto
        {
            Id = asg.Id,
            SlotId = asg.SlotId,
            FechaHoraInicio = asg.FechaHoraInicio,
            FechaHoraFin = asg.FechaHoraFin,
            EmpleadoId = asg.EmpleadoId
        };
    }
}
