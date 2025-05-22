public class AssignmentDto
{
    public Guid Id { get; set; }
    public Guid SlotId { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public DateTime FechaHoraFin{ get; set; }

    public Guid EmpleadoId { get; set; }
}