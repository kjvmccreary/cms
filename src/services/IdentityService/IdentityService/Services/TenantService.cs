using Microsoft.EntityFrameworkCore;
using IdentityService.Data;
using IdentityService.Models;
using Shared.DTOs;

namespace IdentityService.Services
{
    public class TenantService : ITenantService
    {
        private readonly IdentityDbContext _context;
        private readonly ILogger<TenantService> _logger;

        public TenantService(IdentityDbContext context, ILogger<TenantService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<TenantDto>> GetTenantsAsync(PagedRequest request)
        {
            try
            {
                var query = _context.Tenants.AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(t => t.Name.Contains(request.Search) || 
                                           t.Domain.Contains(request.Search));
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply sorting
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    query = request.SortBy.ToLower() switch
                    {
                        "name" => request.SortDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                        "domain" => request.SortDescending ? query.OrderByDescending(t => t.Domain) : query.OrderBy(t => t.Domain),
                        "createdat" => request.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                        _ => query.OrderBy(t => t.Name)
                    };
                }
                else
                {
                    query = query.OrderBy(t => t.Name);
                }

                // Apply pagination
                var tenants = await query
                    .Skip(request.Skip)
                    .Take(request.PageSize)
                    .Select(t => new TenantDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Domain = t.Domain,
                        IsActive = t.IsActive,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToListAsync();

                return new PagedResult<TenantDto>
                {
                    Items = tenants,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tenants");
                return new PagedResult<TenantDto>();
            }
        }

        public async Task<TenantDto?> GetTenantAsync(Guid id)
        {
            try
            {
                var tenant = await _context.Tenants.FindAsync(id);
                if (tenant == null)
                    return null;

                return new TenantDto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Domain = tenant.Domain,
                    IsActive = tenant.IsActive,
                    CreatedAt = tenant.CreatedAt,
                    UpdatedAt = tenant.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tenant {TenantId}", id);
                return null;
            }
        }

        public async Task<TenantDto?> GetTenantByDomainAsync(string domain)
        {
            try
            {
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Domain == domain);

                if (tenant == null)
                    return null;

                return new TenantDto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Domain = tenant.Domain,
                    IsActive = tenant.IsActive,
                    CreatedAt = tenant.CreatedAt,
                    UpdatedAt = tenant.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tenant by domain {Domain}", domain);
                return null;
            }
        }

        public async Task<TenantDto?> CreateTenantAsync(CreateTenantDto createTenantDto)
        {
            try
            {
                // Check if domain already exists
                if (await TenantExistsAsync(createTenantDto.Domain))
                {
                    _logger.LogWarning("Tenant creation failed: Domain {Domain} already exists", createTenantDto.Domain);
                    return null;
                }

                var tenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    Name = createTenantDto.Name,
                    Domain = createTenantDto.Domain,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tenant {TenantName} created successfully with domain {Domain}", 
                    tenant.Name, tenant.Domain);

                return new TenantDto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Domain = tenant.Domain,
                    IsActive = tenant.IsActive,
                    CreatedAt = tenant.CreatedAt,
                    UpdatedAt = tenant.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant {TenantName}", createTenantDto.Name);
                return null;
            }
        }

        public async Task<TenantDto?> UpdateTenantAsync(Guid id, UpdateTenantDto updateTenantDto)
        {
            try
            {
                var tenant = await _context.Tenants.FindAsync(id);
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant update failed: Tenant {TenantId} not found", id);
                    return null;
                }

                tenant.Name = updateTenantDto.Name;
                tenant.IsActive = updateTenantDto.IsActive;
                tenant.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Tenant {TenantId} updated successfully", id);

                return new TenantDto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Domain = tenant.Domain,
                    IsActive = tenant.IsActive,
                    CreatedAt = tenant.CreatedAt,
                    UpdatedAt = tenant.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant {TenantId}", id);
                return null;
            }
        }

        public async Task<bool> DeleteTenantAsync(Guid id)
        {
            try
            {
                var tenant = await _context.Tenants.FindAsync(id);
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant deletion failed: Tenant {TenantId} not found", id);
                    return false;
                }

                // Check if tenant has users
                var hasUsers = await _context.Users.AnyAsync(u => u.TenantId == id);
                if (hasUsers)
                {
                    _logger.LogWarning("Tenant deletion failed: Tenant {TenantId} has associated users", id);
                    return false;
                }

                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Tenant {TenantId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tenant {TenantId}", id);
                return false;
            }
        }

        public async Task<bool> TenantExistsAsync(string domain)
        {
            try
            {
                return await _context.Tenants.AnyAsync(t => t.Domain == domain);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if tenant exists for domain {Domain}", domain);
                return false;
            }
        }
    }
}