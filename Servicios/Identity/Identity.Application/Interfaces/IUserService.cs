
using Identity.Application.Dtos;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);

        Task<UserProfileDto?> GetProfileAsync(Guid userId);
        Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPwd, string newPwd);

        Task<OperationResult> InviteAsync(RegisterDto dto, Guid creatorId);
        Task<OperationResult> CompleteInvitationAsync(RegisterDto dto);
    }

}
