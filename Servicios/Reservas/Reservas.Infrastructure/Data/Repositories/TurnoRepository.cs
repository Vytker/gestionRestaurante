using Microsoft.EntityFrameworkCore;
using Reservas.Application.Interfaces;
using Reservas.Domain.Entities;

namespace Reservas.Infrastructure.Data.Repositories;

public class TurnoRepository : ITurnoRepository
{
    private readonly ReservasDbContext _context;
    public TurnoRepository(ReservasDbContext ctx) => _context = ctx;

    public async Task<IEnumerable<Turno>> ObtenerTodosAsync(Guid? restaurantId) =>
        await _context.Turnos
                       .Include(t => t.Reservas)
                      .Where(t => !t.Eliminado && t.RestauranteId == restaurantId)
                      .ToListAsync();

    public async Task<Turno?> ObtenerPorIdAsync(int id, Guid restaurantId) =>
        await _context.Turnos
                      .Include(t => t.Reservas)
                      .FirstOrDefaultAsync(t => t.Id == id &&
                                                t.RestauranteId == restaurantId &&
                                                !t.Eliminado);

    public void Crear(Turno turno) => _context.Turnos.Add(turno);
    public void Actualizar(Turno turno) => _context.Turnos.Update(turno);
    public void Eliminar(Turno turno) => _context.Turnos.Remove(turno);


    public Task GuardarCambiosAsync() => _context.SaveChangesAsync();
}
