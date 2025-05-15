// Turnos.Domain/Entities/Slot.cs
using System;
using Turnos.Domain.ValueObjects;

namespace Turnos.Domain.Entities
{
    public class Slot
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public IntervaloTiempo Horario { get; private set; }
        public Guid OwnerId { get; private set; }

        private Slot() { } // EF

        public Slot(string name, IntervaloTiempo horario, Guid ownerId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Horario = horario;
            OwnerId = ownerId;
        }

        public void Update(string name, IntervaloTiempo horario)
        {
            Name = name;
            Horario = horario;
        }
    }
}
