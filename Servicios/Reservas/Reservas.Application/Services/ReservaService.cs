using Microsoft.EntityFrameworkCore;
using Reservas.Application.Interfaces;
using Reservas.Infrastructure.Data;

namespace Reservas.Application.Services
{
    
        public class ReservaService : IReservaService
        {
            private readonly ReservasDbContext _dbContext;

            public ReservaService(ReservasDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public IEnumerable<Domain.Entities.Reserva> ObtenerTodasReservas()
            {
                return _dbContext.Reservas.ToList();
            }
            public void CrearReserva(Domain.Entities.Reserva reserva)
            {
                // Solución: Generar un nuevo Id basado en el conteo de registros en la base de datos
                if (reserva.FechaReserva.Kind == DateTimeKind.Unspecified)
                {
                    reserva.FechaReserva = DateTime.SpecifyKind(reserva.FechaReserva, DateTimeKind.Utc);
                }
                _dbContext.Reservas.Add(reserva);
                _dbContext.SaveChanges();
            }
        }
    }

