using System.ComponentModel.DataAnnotations;

namespace Reservas.Application.Dtos
{
    public class TurnoCreateDto
    {
        [Required(ErrorMessage = "El nombre del turno es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del turno no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty; // Ejemplo: "Turno de la mañana"

        [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
        [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "La hora de inicio debe estar entre 00:00 y 23:59:59.")]
        public TimeSpan HoraInicio { get; set; } // Hora de inicio del turno

        [Required(ErrorMessage = "La hora de fin es obligatoria.")]
        [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "La hora de fin debe estar entre 00:00 y 23:59:59.")]
        public TimeSpan HoraFin { get; set; } // Hora de fin del turno

        [Required(ErrorMessage = "La capacidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a 0.")]
        public int Capacidad { get; set; } // Capacidad máxima de reservas para este turno

        // Solo obligatorio para SuperAdmin
        public Guid RestauranteId { get; set; }
    }
}

