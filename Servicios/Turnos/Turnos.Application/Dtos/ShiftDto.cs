using System;

namespace Turnos.Application.Dtos
{
    public class ShiftDto
    {
        public Guid Id { get; set; }
        public Guid EmpleadoId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
    }
}
