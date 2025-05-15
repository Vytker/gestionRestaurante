using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Turnos.Domain.Entities;
using Turnos.Infrastructure.Persistence;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, AssignmentDto>
{
    private readonly TurnosDbContext _context;
    public CreateAssignmentCommandHandler(TurnosDbContext ctx) => _context = ctx;

    public async Task<AssignmentDto> Handle(CreateAssignmentCommand req, CancellationToken ct)
    {
        var asg = new Assignment(req.SlotId, req.Date, req.EmpleadoId, req.OwnerId);
        _context.Assignments.Add(asg);
        await _context.SaveChangesAsync(ct);

        return new AssignmentDto
        {
            Id = asg.Id,
            SlotId = asg.SlotId,
            Date = asg.Date,
            EmpleadoId = asg.EmpleadoId
        };
    }
}
