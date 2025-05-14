
namespace Identity.Application.Dtos
{
    public record UpdateProfileDto
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? Telefono { get; init; }
    }

}
