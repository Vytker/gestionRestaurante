// Turnos.Domain/Entities/Assignment.cs
using System;

namespace Turnos.Domain.Entities
{
    public class Assignment
    {
        public Guid Id { get; private set; }
        public Guid SlotId { get; private set; }
        public DateTime FechaHoraInicio { get; private set; }
        public DateTime FechaHoraFin { get; private set; }
        public Guid EmpleadoId { get; private set; }
        public Guid OwnerId { get; private set; }

        private Assignment() { }

        public Assignment(Guid slotId, DateTime fechaHoraInicio, DateTime fechaHoraFin, Guid empleadoId, Guid ownerId)
        {
            Id = Guid.NewGuid();
            SlotId = slotId;
            FechaHoraInicio = fechaHoraInicio;
            FechaHoraFin = fechaHoraFin;
            EmpleadoId = empleadoId;
            OwnerId = ownerId;
        }
    }
}
