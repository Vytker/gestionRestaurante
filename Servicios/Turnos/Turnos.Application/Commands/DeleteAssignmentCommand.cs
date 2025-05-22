using MediatR;
public class DeleteAssignmentCommand : IRequest<Unit>
{
    public Guid Id { get; }
    public DeleteAssignmentCommand(Guid id) => Id = id;
}