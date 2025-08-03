using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IdentityService.Services;
using Shared.DTOs;
using Shared.Infrastructure;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ITenantContext tenantContext,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);
            
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(createUserDto);
            
            if (user == null)
                return BadRequest(new { message = "Registration failed. User may already exist." });

            return CreatedAtAction(nameof(GetCurrentUser), new { }, user);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<string>> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var newToken = await _authService.RefreshTokenAsync(refreshToken);
            
            if (newToken == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            return Ok(new { token = newToken });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token not found" });

            await _authService.LogoutAsync(token);
            return Ok(new { message = "Logged out successfully" });
        }


        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _authService.GetCurrentUserAsync(); // No parameters

            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _tenantContext.UserId;
            var success = await _authService.ChangePasswordAsync(
                userId, 
                changePasswordDto.CurrentPassword, 
                changePasswordDto.NewPassword);

            if (!success)
                return BadRequest(new { message = "Failed to change password. Current password may be incorrect." });

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.ResetPasswordAsync(resetPasswordDto.Email);
            
            // Always return success for security reasons
            return Ok(new { message = "If the email exists, a password reset link has been sent." });
        }
    }

    // Additional DTOs for Auth endpoints
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }
}