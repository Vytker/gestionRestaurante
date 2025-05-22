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
        public Guid RestauranteId { get; private set; }

        public bool IsDeleted { get; set; } = false; //soft delete
        private Slot() { } // EF

        public Slot(string name, IntervaloTiempo horario, Guid restauranteId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Horario = horario;
            RestauranteId = restauranteId;
        }

        public void Update(string name, IntervaloTiempo horario)
        {
            Name = name;
            Horario = horario;
        }
    }
}
