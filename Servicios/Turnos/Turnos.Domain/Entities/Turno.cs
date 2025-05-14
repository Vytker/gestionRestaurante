using System;
using Turnos.Domain.ValueObjects;

namespace Turnos.Domain.Entities
{
    public class Turno
    {
        public Guid Id { get; private set; }
        public Guid EmpleadoId { get; private set; }
        public Guid OwnerId { get; private set; }
        public IntervaloTiempo Horario { get; private set; }

        private Turno() { }

        public Turno(Guid empleadoId, Guid ownerId, IntervaloTiempo horario)
        {
            if (horario == null) throw new ArgumentNullException(nameof(horario));
            Id = Guid.NewGuid();
            EmpleadoId = empleadoId;
            OwnerId = ownerId;
            Horario = horario;
        }

        public bool SolapaCon(Turno otro)
        {
            return Horario.SolapaCon(otro.Horario);
        }
    }
}
