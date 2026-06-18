public class OperationResult
{
    public bool Success { get; init; }
    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();
    public static OperationResult Ok() => new() { Success = true };
    public static OperationResult Fail(params string[] errors)
        => new() { Success = false, Errors = errors };
}