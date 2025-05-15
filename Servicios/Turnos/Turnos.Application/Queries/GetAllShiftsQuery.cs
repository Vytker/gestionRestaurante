// Turnos.Application/Queries/GetAllShiftsQuery.cs
using MediatR;
using System;
using System.Collections.Generic;
using Turnos.Application.Dtos;

namespace Turnos.Application.Queries
{
    /// <summary>
    /// Devuelve todos los turnos, opcionalmente filtrados por fecha (fecha exacta).
    /// </summary>
    public class GetAllShiftsQuery : IRequest<IEnumerable<ShiftDto>>
    {
        /// <summary>
        /// Si se suministra, devolverá solo los turnos cuyo inicio
        /// o fin caigan en este día.
        /// </summary>
        public DateTime? Day { get; }

        public GetAllShiftsQuery(DateTime? day = null)
            => Day = day;
    }
}
