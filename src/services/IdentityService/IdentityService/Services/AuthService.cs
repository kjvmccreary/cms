using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using IdentityService.Data;
using IdentityService.Models;
using Shared.DTOs;

namespace IdentityService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IdentityDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IdentityDbContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Tenant)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || !user.IsActive || !user.Tenant.IsActive)
                {
                    _logger.LogWarning("Login failed for email {Email}: User not found or inactive", loginDto.Email);
                    return null;
                }

                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed for email {Email}: Invalid password", loginDto.Email);
                    return null;
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    TenantId = user.TenantId,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray()
                };

                _logger.LogInformation("User {Email} logged in successfully", user.Email);
                
                return new LoginResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes()),
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", loginDto.Email);
                return null;
            }
        }

        public async Task<UserDto?> RegisterAsync(CreateUserDto createUserDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .AnyAsync(u => u.Email == createUserDto.Email);

                if (existingUser)
                {
                    _logger.LogWarning("Registration failed for email {Email}: User already exists", createUserDto.Email);
                    return null;
                }

                // For now, we'll assign to the first active tenant
                // In a real scenario, this would be handled differently
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.IsActive);

                if (tenant == null)
                {
                    _logger.LogError("No active tenant found for user registration");
                    return null;
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = createUserDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    TenantId = tenant.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);

                // Assign default role
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (userRole != null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = userRole.Id,
                        AssignedAt = DateTime.UtcNow,
                        AssignedBy = user.Id // Self-assigned during registration
                    });
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Email} registered successfully", user.Email);

                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    TenantId = user.TenantId,
                    Roles = createUserDto.Roles ?? new[] { "User" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email {Email}", createUserDto.Email);
                return null;
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

                if (user == null)
                    return null;

                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    TenantId = user.TenantId,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || !user.IsActive)
                    return false;

                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                    return false;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Password changed for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return false;
            }
        }

        public async Task<string?> RefreshTokenAsync(string refreshToken)
        {
            // TODO: Implement refresh token logic with proper storage
            // For now, return null - this would typically involve validating
            // the refresh token and generating a new access token
            await Task.CompletedTask;
            return null;
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            // TODO: Implement logout logic (invalidate tokens, etc.)
            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            // TODO: Implement password reset logic
            await Task.CompletedTask;
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new("tenant_id", user.TenantId.ToString()),
                new("tenant_domain", user.Tenant?.Domain ?? string.Empty)
            };

            // Add role claims
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private int GetJwtExpirationMinutes()
        {
            return _configuration.GetValue<int>("Jwt:ExpirationInMinutes", 60);
        }
    }
}