

namespace Reservas.Application.Dtos
{
    public class TurnoCreateDto
    {
        public string Nombre { get; set; } = string.Empty; // Nombre del turno (ej. "Turno de la mañana")
        public TimeSpan HoraInicio { get; set; } // Hora de inicio del turno
        public TimeSpan HoraFin { get; set; } // Hora de fin del turno
        public int Capacidad { get; set; } // Capacidad máxima de reservas para este turno
    }
}
