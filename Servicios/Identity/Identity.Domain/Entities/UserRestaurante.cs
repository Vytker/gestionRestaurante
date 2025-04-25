
namespace Identity.Domain.Entities
{
    public class UserRestaurante
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid RestaurantId { get; set; }
        public Restaurante Restaurante { get; set; } = null!;

        public string Role { get; set; } = "Staff";  // "Owner" | "Staff"
    }
}
