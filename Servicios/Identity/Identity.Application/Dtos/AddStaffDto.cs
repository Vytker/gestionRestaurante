
namespace Identity.Application.Dtos
{
    public class AddStaffDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = "Staff";   // "Staff" o "Owner" (si delegas)
    }
}
