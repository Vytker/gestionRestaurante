
// This file is part of the Reservas project.
using System.ComponentModel.DataAnnotations;

namespace Reservas.Application.Dtos
{
    public class ReservaCreateDto
    {
        [Required(ErrorMessage ="El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder los 100 caracteres.")]
        public string NombreCliente { get; set; } = string.Empty;
        [Required(ErrorMessage = "El email del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El email del cliente no puede exceder los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "La fecha de reserva es obligatoria.")]

        public DateTime FechaReserva { get; set; }
        [Required(ErrorMessage = "El número de comensales es obligatorio.")]
        [Range(1, 100, ErrorMessage = "El número de comensales debe estar entre 1 y 100.")]

        public int NumeroComensales { get; set; }
        [StringLength(500, ErrorMessage = "Las notas no pueden exceder los 500 caracteres.")]
        public string? Notas { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un turno.")]
        public int TurnoId { get; set; } // ID del turno al que pertenece la reserva

    }
}
//porque usar un dto? esto da mas control ya que hay propiedades que no se deben exponer al cliente 
// seguridad claridad y control sobre los datos que entran
// y salen de la aplicacion
