using System;

namespace Turnos.Domain.ValueObjects
{
    public class IntervaloTiempo
    {
        public DateTime Inicio { get; }
        public DateTime Fin { get; }

        public IntervaloTiempo(DateTime inicio, DateTime fin)
        {
            if (fin <= inicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la de inicio.");

            Inicio = inicio;
            Fin = fin;
        }

        public bool SolapaCon(IntervaloTiempo otro)
        {
            return Inicio < otro.Fin && otro.Inicio < Fin;
        }
    }
}
