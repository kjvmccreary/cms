using Shared.DTOs;

namespace IdentityService.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<UserDto?> RegisterAsync(CreateUserDto createUserDto);
        Task<UserDto?> GetCurrentUserAsync();
        Task<bool> ValidateTokenAsync(string token);
        Task<string?> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string token);
        Task<bool> ResetPasswordAsync(string email);
        Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    }
}