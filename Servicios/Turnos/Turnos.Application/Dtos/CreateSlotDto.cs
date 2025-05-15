
public class CreateSlotDto
{
    public string Name { get; set; } = null!;
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
}
