using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reservas.Domain.Entities
{
    public class Reserva
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string NombreCliente { get; set; }
        
        public required DateTime FechaReserva { get; set; }
        public required int NumeroComensales { get; set; }

        [StringLength(50)]
        public string? Notas { get; set; }


    }
}
