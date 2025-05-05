using Reservas.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace Reservas.Application.Dtos
{
    public class ReservaUpdateDto
    {
        [Required]
        [FutureDateHours(1, ErrorMessage = "La reserva debe hacerse con al menos 1 hora de antelación.")]
        public required DateTime FechaReserva { get; set; }
        [Required, Range(1, 20, ErrorMessage = "Número de comensales entre 1 y 20.")]
        public required int NumeroComensales { get; set; }
    }
}
