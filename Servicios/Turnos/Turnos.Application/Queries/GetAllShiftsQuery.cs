using MediatR;
using System;
using System.Collections.Generic;
using Turnos.Application.Dtos;

namespace Turnos.Application.Queries
{
    
    /// Devuelve todos los turnos, opcionalmente filtrados por fecha (fecha exacta).
    
    public class GetAllShiftsQuery : IRequest<IEnumerable<ShiftDto>>
    {
        
        /// Si se suministra, devolverá solo los turnos cuyo inicio
        /// o fin caigan en este día.
        
        public DateTime? Day { get; }

        public GetAllShiftsQuery(DateTime? day = null)
            => Day = day;
    }
}
