namespace Reservas.Application.Dtos
{
    public class ReservaUpdateDto
    {
        public required DateTime FechaReserva { get; set; }
        public required int NumeroComensales { get; set; }
    }
}
