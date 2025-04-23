
namespace Reservas.Application.Dtos
{
    public class TurnoUpdateDto
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; } = string.Empty;
        public int? Capacidad { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        
        
    }
}
