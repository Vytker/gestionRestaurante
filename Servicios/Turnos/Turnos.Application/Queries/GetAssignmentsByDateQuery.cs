using MediatR;
using System;
using System.Collections.Generic;
using Turnos.Application.Dtos;

public class GetAssignmentsByDateQuery : IRequest<IEnumerable<AssignmentDto>>
{
    public DateTime Date { get; }
    public GetAssignmentsByDateQuery(DateTime date) => Date = date.Date;
}