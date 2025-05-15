
public class SlotDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
}
