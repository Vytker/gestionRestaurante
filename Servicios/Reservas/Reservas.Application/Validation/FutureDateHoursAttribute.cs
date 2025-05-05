
using System.ComponentModel.DataAnnotations;

namespace Reservas.Application.Validation
{
    public class FutureDateHoursAttribute : ValidationAttribute
    {
        private readonly int _minHoursInFuture;
        public FutureDateHoursAttribute(int minHoursInFuture)
        {
            _minHoursInFuture = minHoursInFuture;
        }

        public override bool IsValid(object? value)
        {
            if (value is DateTime dt)
            {
                return dt.ToUniversalTime() > DateTime.UtcNow.AddHours(_minHoursInFuture);
            }
            return false;
        }
    }
}
