using Shared.DTOs;

namespace IdentityService.Services
{
    public interface ITenantService
    {
        Task<PagedResult<TenantDto>> GetTenantsAsync(PagedRequest request);
        Task<TenantDto?> GetTenantAsync(Guid id);
        Task<TenantDto?> GetTenantByDomainAsync(string domain);
        Task<TenantDto?> CreateTenantAsync(CreateTenantDto createTenantDto);
        Task<TenantDto?> UpdateTenantAsync(Guid id, UpdateTenantDto updateTenantDto);
        Task<bool> DeleteTenantAsync(Guid id);
        Task<bool> TenantExistsAsync(string domain);
    }
}