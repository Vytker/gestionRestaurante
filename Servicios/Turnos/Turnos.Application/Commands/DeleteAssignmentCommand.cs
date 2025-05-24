using MediatR;
using Turnos.Application.Common;
public class DeleteAssignmentCommand : IRequest<Unit>, ITenantScoped
{
    public Guid Id { get; }
    public Guid RestauranteId { get; set; }
    public DeleteAssignmentCommand(Guid id) => Id = id;
}