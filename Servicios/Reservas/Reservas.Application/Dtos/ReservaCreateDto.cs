
// This file is part of the Reservas project.
namespace Reservas.Application.Dtos
{
    public class ReservaCreateDto
    {
        public string NombreCliente { get; set; } = string.Empty;
        public DateTime FechaReserva { get; set; }
        public int NumeroComensales { get; set; }
        public string? Notas { get; set; }
        public int TurnoId { get; set; } // ID del turno al que pertenece la reserva

    }
}
//porque usar un dto? esto da mas control ya que hay propiedades que no se deben exponer al cliente 
// seguridad claridad y control sobre los datos que entran
// y salen de la aplicacion
