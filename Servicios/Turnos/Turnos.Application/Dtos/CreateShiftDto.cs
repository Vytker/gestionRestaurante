// Turnos.Application/Dtos/CreateShiftDto.cs
using System;

namespace Turnos.Application.Dtos
{
    public class CreateShiftDto
    {
        public Guid EmployeeId { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
