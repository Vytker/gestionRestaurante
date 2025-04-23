namespace Reservas.Application.Dtos
{
    public class ReservaDto
    {
        public Guid Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public DateTime FechaReserva { get; set; }
        public int NumeroComensales { get; set; }
        public string? Notas { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int TurnoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
    }
}
