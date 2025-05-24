using MediatR;
using System;
using System.Collections.Generic;
using Turnos.Application.Common;
using Turnos.Application.Dtos;

public class GetAssignmentsByDateQuery : IRequest<IEnumerable<AssignmentDto>>, ITenantScoped
{
    public DateTime Date { get; }
    public Guid RestauranteId { get; set; }
    public GetAssignmentsByDateQuery(DateTime date) => Date = date.Date;
}