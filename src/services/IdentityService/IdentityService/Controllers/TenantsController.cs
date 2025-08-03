using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IdentityService.Services;
using Shared.DTOs;
using Shared.Infrastructure;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(
            ITenantService tenantService,
            ITenantContext tenantContext,
            ILogger<TenantsController> logger)
        {
            _tenantService = tenantService;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<TenantDto>>> GetTenants([FromQuery] PagedRequest request)
        {
            var result = await _tenantService.GetTenantsAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TenantDto>> GetTenant(Guid id)
        {
            // Users can only view their own tenant unless they're admin
            if (!_tenantContext.UserRoles.Contains("Admin") && id != _tenantContext.TenantId)
            {
                return Forbid();
            }

            var tenant = await _tenantService.GetTenantAsync(id);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [HttpGet("by-domain/{domain}")]
        [AllowAnonymous] // This might be needed for tenant resolution
        public async Task<ActionResult<TenantDto>> GetTenantByDomain(string domain)
        {
            var tenant = await _tenantService.GetTenantByDomainAsync(domain);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TenantDto>> CreateTenant(CreateTenantDto createTenantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenant = await _tenantService.CreateTenantAsync(createTenantDto);
            if (tenant == null)
                return BadRequest(new { message = "Failed to create tenant. Domain may already exist." });

            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TenantDto>> UpdateTenant(Guid id, UpdateTenantDto updateTenantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenant = await _tenantService.UpdateTenantAsync(id, updateTenantDto);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var success = await _tenantService.DeleteTenantAsync(id);
            if (!success)
                return BadRequest(new { message = "Failed to delete tenant. Tenant may have associated users." });

            return NoContent();
        }

        [HttpGet("current")]
        public async Task<ActionResult<TenantDto>> GetCurrentTenant()
        {
            var tenantId = _tenantContext.TenantId;
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        [HttpGet("check-domain/{domain}")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CheckDomainExists(string domain)
        {
            var exists = await _tenantService.TenantExistsAsync(domain);
            return Ok(new { exists });
        }
    }
}