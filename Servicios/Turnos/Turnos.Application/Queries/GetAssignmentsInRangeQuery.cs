
using MediatR;
using Turnos.Application.Common;

public class GetAssignmentsInRangeQuery : IRequest<IEnumerable<AssignmentDto>>, ITenantScoped
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public Guid RestauranteId { get; set; }
    public GetAssignmentsInRangeQuery(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
}
