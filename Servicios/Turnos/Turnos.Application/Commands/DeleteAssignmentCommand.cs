using MediatR;
public class DeleteAssignmentCommand : IRequest<Unit>
{
    public Guid Id { get; }

    public Guid? OwnerId { get; }  // null cuando lo invoca SuperAdmin
    public bool IsSuperAdmin { get; }  // true → omitir validación de OwnerId
    public DeleteAssignmentCommand(Guid id, Guid? ownerId, bool isSuperAdmin)
    {
        Id = id;
        OwnerId = ownerId;
        IsSuperAdmin = isSuperAdmin;
    }
}