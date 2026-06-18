// Identity.Domain.Entities/Invitation.cs
namespace Identity.Domain.Entities
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }       // El usuario al que se invitó
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        // Navegación opcional al usuario
        public User User { get; set; } = null!;
    }
}
