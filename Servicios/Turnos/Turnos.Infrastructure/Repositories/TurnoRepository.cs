using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Turnos.Domain.Entities;
using Turnos.Domain.ValueObjects;
using Turnos.Infrastructure.Persistence;

namespace Turnos.Infrastructure.Repositories
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly TurnosDbContext _context;
        public TurnoRepository(TurnosDbContext context) => _context = context;

        public async Task<IEnumerable<Turno>> GetByEmpleadoAsync(Guid empleadoId, IntervaloTiempo intervalo)
        {
            return await _context.Turnos
                .Where(t => t.EmpleadoId == empleadoId
                         && t.Horario.Inicio >= intervalo.Inicio
                         && t.Horario.Fin <= intervalo.Fin)
                .ToListAsync();
        }

        public async Task AddAsync(Turno turno)
        {
            await _context.Turnos.AddAsync(turno);
            await _context.SaveChangesAsync();
        }
    }
}
