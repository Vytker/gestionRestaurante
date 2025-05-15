using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Turnos.Application.Commands;
using Turnos.Infrastructure.Persistence;

namespace Turnos.Application.Handlers
{
    public class DeleteShiftCommandHandler
        : IRequestHandler<DeleteShiftCommand, Unit>
    {
        private readonly TurnosDbContext _context;

        public DeleteShiftCommandHandler(TurnosDbContext context)
            => _context = context;

        public async Task<Unit> Handle(
            DeleteShiftCommand request,
            CancellationToken cancellationToken)
        {
            var turno = await _context.Turnos
                .FirstOrDefaultAsync(t => t.Id == request.ShiftId, cancellationToken);

            if (turno == null)
                throw new InvalidOperationException("Turno no encontrado.");

            if (turno.OwnerId != request.OwnerId)
                throw new UnauthorizedAccessException("No puedes eliminar este turno.");

            _context.Turnos.Remove(turno);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
