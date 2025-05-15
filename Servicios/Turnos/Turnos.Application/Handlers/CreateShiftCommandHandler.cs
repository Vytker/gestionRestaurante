using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Turnos.Application.Commands;
using Turnos.Application.Dtos;
using Turnos.Domain.Entities;
using Turnos.Domain.ValueObjects;
using Turnos.Infrastructure.Persistence;

namespace Turnos.Application.Handlers
{
    public class CreateShiftCommandHandler : IRequestHandler<CreateShiftCommand, ShiftDto>
    {
        private readonly TurnosDbContext _context;

        public CreateShiftCommandHandler(TurnosDbContext context)
            => _context = context;

        public async Task<ShiftDto> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
        {
            // 1) Validar intervalo
            var intervalo = new IntervaloTiempo(request.Start, request.End);

            // 2) Comprobar solapamiento para el mismo empleado
            var conflict = await _context.Turnos
                .AnyAsync(t =>
                    t.EmpleadoId == request.EmployeeId &&
                    t.Horario.Inicio < request.End &&
                    t.Horario.Fin > request.Start,
                    cancellationToken);

            if (conflict)
                throw new InvalidOperationException("El turno se solapa con otro existente.");

            // 3) Crear entidad
            var turno = new Turno(request.EmployeeId, request.OwnerId, intervalo);
            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync(cancellationToken);

            // 4) Mapear a DTO de salida
            return new ShiftDto
            {
                Id = turno.Id,
                EmpleadoId = turno.EmpleadoId,
                FechaHoraInicio = turno.Horario.Inicio,
                FechaHoraFin = turno.Horario.Fin
            };
        }
    }
}
