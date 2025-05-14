using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Turnos.Domain.Entities;
using Turnos.Domain.ValueObjects;

namespace Turnos.Infrastructure.Repositories
{
    public interface ITurnoRepository
    {
        Task<IEnumerable<Turno>> GetByEmpleadoAsync(Guid empleadoId, IntervaloTiempo intervalo);
        Task AddAsync(Turno turno);
        // ... otros métodos (UpdateAsync, DeleteAsync) si los necesitas
    }
}
