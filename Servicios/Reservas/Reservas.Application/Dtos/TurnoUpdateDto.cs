using System.ComponentModel.DataAnnotations;

namespace Reservas.Application.Dtos
{
    public class TurnoUpdateDto
    {
        public int? Id { get; set; }

        [StringLength(100, ErrorMessage = "El nombre del turno no puede exceder los 100 caracteres.")]
        public string? Nombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a 0.")]
        public int? Capacidad { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "La hora de inicio debe estar entre 00:00 y 23:59:59.")]
        public TimeSpan? HoraInicio { get; set; }

        [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "La hora de fin debe estar entre 00:00 y 23:59:59.")]
        public TimeSpan? HoraFin { get; set; }
    }
}
