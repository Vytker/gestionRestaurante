
namespace Identity.Application.Dtos
{
    public class AddStaffDto
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Staff";   // "Staff" o "Owner" (si delegas)
    }
}
