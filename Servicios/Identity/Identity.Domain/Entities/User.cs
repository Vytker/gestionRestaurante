

namespace Identity.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Telefono { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? Role { get; set; }
        public ICollection<UserRestaurante> Restaurantes { get; set; } = new List<UserRestaurante>();


    }
}
