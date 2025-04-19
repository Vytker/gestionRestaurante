
using Identity.Application.Dtos;

namespace Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
