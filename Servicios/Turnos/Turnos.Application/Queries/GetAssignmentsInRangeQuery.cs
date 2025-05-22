
using MediatR;

public class GetAssignmentsInRangeQuery : IRequest<IEnumerable<AssignmentDto>>
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public GetAssignmentsInRangeQuery(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
}
