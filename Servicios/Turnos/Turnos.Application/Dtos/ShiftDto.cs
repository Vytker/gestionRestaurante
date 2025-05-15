using System;

namespace Turnos.Application.Dtos
{
    public class ShiftDto
    {
        public Guid Id { get; set; }
        public Guid EmpleadoId { get; set; }
        public TimeSpan FechaHoraInicio { get; set; }
        public TimeSpan FechaHoraFin { get; set; }
    }
}
