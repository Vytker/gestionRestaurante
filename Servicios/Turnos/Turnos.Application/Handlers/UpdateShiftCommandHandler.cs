using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Turnos.Application.Commands;
using Turnos.Domain.ValueObjects;
using Turnos.Infrastructure.Persistence;

namespace Turnos.Application.Handlers
{
    public class UpdateShiftCommandHandler : IRequestHandler<UpdateShiftCommand, Unit>
    {
        private readonly TurnosDbContext _context;

        public UpdateShiftCommandHandler(TurnosDbContext context)
            => _context = context;

        public async Task<Unit> Handle(UpdateShiftCommand request, CancellationToken cancellationToken)
        {
            var turno = await _context.Turnos
                .FirstOrDefaultAsync(t => t.Id == request.ShiftId, cancellationToken);

            if (turno == null)
                throw new InvalidOperationException("Turno no encontrado.");

            if (turno.OwnerId != request.OwnerId)
                throw new UnauthorizedAccessException("No puedes editar este turno.");

            // Validar nuevo intervalo
            var interval = new IntervaloTiempo(request.Start, request.End);

            // Comprobar solapamientos (excluyendo el propio turno)
            bool conflict = await _context.Turnos
                .AnyAsync(t =>
                    t.Id != request.ShiftId &&
                    t.EmpleadoId == turno.EmpleadoId &&
                    t.Horario.Inicio < request.End &&
                    t.Horario.Fin > request.Start,
                    cancellationToken);

            if (conflict)
                throw new InvalidOperationException("El nuevo horario se solapa con otro turno.");

            // Aplicar cambios
            turno.UpdateHorario(interval);   // Asumiendo que expones un método en la entidad
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
