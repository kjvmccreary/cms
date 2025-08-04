using ContractService.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ContractService.Data
{
    /// <summary>
    /// Health check for the ContractDbContext database connection
    /// </summary>
    public class ContractDbContextHealthCheck : IHealthCheck
    {
        private readonly ContractDbContext _context;

        public ContractDbContextHealthCheck(ContractDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Test database connection
                await _context.Database.CanConnectAsync(cancellationToken);
                
                return HealthCheckResult.Healthy("Database connection is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database connection failed", ex);
            }
        }
    }
}