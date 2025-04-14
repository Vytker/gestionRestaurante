

namespace Reservas.Application.Interfaces
{
    public interface IReservaService
    {
        IEnumerable<Reservas.Domain.Entities.Reserva> ObtenerTodasReservas();
        void CrearReserva(Reservas.Domain.Entities.Reserva reserva);
    }
}
