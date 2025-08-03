using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shared.Infrastructure
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
        {
            // Extract tenant information from JWT token
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tenantDomainClaim = context.User.FindFirst("tenant_domain")?.Value;
                var emailClaim = context.User.FindFirst(ClaimTypes.Email)?.Value;
                var rolesClaim = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

                if (Guid.TryParse(tenantIdClaim, out var tenantId) && 
                    Guid.TryParse(userIdClaim, out var userId))
                {
                    var mutableContext = (TenantContext)tenantContext;
                    mutableContext.TenantId = tenantId;
                    mutableContext.UserId = userId;
                    mutableContext.TenantDomain = tenantDomainClaim ?? string.Empty;
                    mutableContext.UserEmail = emailClaim ?? string.Empty;
                    mutableContext.UserRoles = rolesClaim;
                    mutableContext.IsAuthenticated = true;
                }
            }

            await _next(context);
        }
    }
}