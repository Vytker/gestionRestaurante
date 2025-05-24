using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Turnos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class DeleteAssignmentCommandHandler : IRequestHandler<DeleteAssignmentCommand, Unit>
{
    private readonly TurnosDbContext _context;
    public DeleteAssignmentCommandHandler(TurnosDbContext ctx) => _context = ctx;

    public async Task<Unit> Handle(DeleteAssignmentCommand req, CancellationToken ct)
    {
        var asg = await _context.Assignments
            .FirstOrDefaultAsync(a => a.Id == req.Id && a.RestauranteId == req.RestauranteId , ct)
            ?? throw new InvalidOperationException("Assignment not found");
        _context.Assignments.Remove(asg);
        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}