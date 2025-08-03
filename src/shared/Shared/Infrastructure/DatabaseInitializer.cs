using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Infrastructure
{
    public class DatabaseInitializer<TContext> : IHostedService where TContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializer<TContext>> _logger;

        public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer<TContext>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                _logger.LogInformation("Starting database migration for {Context}", typeof(TContext).Name);
                await context.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database migration completed for {Context}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while migrating the database for {Context}", typeof(TContext).Name);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}