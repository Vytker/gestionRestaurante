namespace Identity.Application.Dtos
{
    public class RegisterDto
    {
        // Para invitación (Owner/SuperAdmin)
        public string? Email { get; set; }
        public string? Role { get; set; } = "User";

        // → Datos comunes
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PasswordConfirm { get; set; }

        // Para completado (primer login)
        public string? InviteToken { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Guid? RestaurantId { get; set; }
    }
}
