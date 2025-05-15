// Turnos.Domain/Entities/Assignment.cs
using System;

namespace Turnos.Domain.Entities
{
    public class Assignment
    {
        public Guid Id { get; private set; }
        public Guid SlotId { get; private set; }
        public DateTime Date { get; private set; }        // solo fecha parte
        public Guid EmpleadoId { get; private set; }
        public Guid OwnerId { get; private set; }

        private Assignment() { }

        public Assignment(Guid slotId, DateTime date, Guid empleadoId, Guid ownerId)
        {
            Id = Guid.NewGuid();
            SlotId = slotId;
            Date = date.Date;
            EmpleadoId = empleadoId;
            OwnerId = ownerId;
        }
    }
}
