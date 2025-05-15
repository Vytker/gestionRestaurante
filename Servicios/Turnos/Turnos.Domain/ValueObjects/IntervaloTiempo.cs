using System;
using Microsoft.EntityFrameworkCore;
namespace Turnos.Domain.ValueObjects
{
    [Owned]
    public class IntervaloTiempo
    {
        public TimeSpan Inicio { get; private set; }
        public TimeSpan Fin { get; private set; }

        public IntervaloTiempo(TimeSpan inicio, TimeSpan fin)
        {
            Inicio = inicio;
            Fin = fin;
        }
        private IntervaloTiempo() { }
        public bool SolapaCon(IntervaloTiempo otro)
        {
            return Inicio < otro.Fin && otro.Inicio < Fin;
        }
    }
}
