

namespace Reservas.Domain.Entities
{
    public class Turno
    {
        public int Id { get; set; }

        public Guid RestauranteId { get; set; } // Clave foránea para el restaurante al que pertenece el turno
        public required string Nombre { get; set; } = null!; //"Turno de la mañana", "Turno de la tarde", etc.

        public required TimeSpan HoraInicio { get; set; } // Hora de inicio del turno

        public required TimeSpan HoraFin { get; set; } // Hora de fin del turno

        public required int Capacidad { get; set; } // Capacidad máxima de reservas para este turno

        public bool Eliminado { get; set; } = false; // Indica si el turno ha sido eliminado lógicamente

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>(); // Relación con reservas
        
    }
}
