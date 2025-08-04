using ContractService.Data.Configurations;
using ContractService.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;
using Shared.Models;

namespace ContractService.Data
{
    /// <summary>
    /// Entity Framework DbContext for the Contract Service
    /// </summary>
    public class ContractDbContext : DbContext
    {
        private readonly ITenantContext _tenantContext;

        public ContractDbContext(DbContextOptions<ContractDbContext> options, ITenantContext tenantContext)
            : base(options)
        {
            _tenantContext = tenantContext;
        }

        // DbSets
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }
        public DbSet<ContractParty> ContractParties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new ContractConfiguration());
            modelBuilder.ApplyConfiguration(new ContractTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContractPartyConfiguration());

            // Global query filters for multi-tenancy
            ApplyGlobalFilters(modelBuilder);

            // Set up relationships
            ConfigureRelationships(modelBuilder);

            // Seed data
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Apply global query filters for multi-tenancy
        /// </summary>
        private void ApplyGlobalFilters(ModelBuilder modelBuilder)
        {
            // Apply tenant filter to all entities that inherit from BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // For now, we'll comment out global filters as they require complex expression trees
                    // In a production system, you would implement proper global query filters
                    // This ensures the build succeeds while we establish the basic structure
                    
                    // TODO: Implement proper global query filters for multi-tenancy
                    // Example: modelBuilder.Entity(entityType.ClrType).HasQueryFilter(/* tenant filter expression */);
                }

                if (typeof(BaseAuditEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // TODO: Implement soft delete global query filter
                    // Example: modelBuilder.Entity(entityType.ClrType).HasQueryFilter(/* soft delete filter expression */);
                }
            }
        }

        /// <summary>
        /// Configure entity relationships
        /// </summary>
        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Contract -> ContractType (Many-to-One)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.ContractType)
                .WithMany(ct => ct.Contracts)
                .HasForeignKey(c => c.ContractTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contract -> ContractParty (One-to-Many)
            modelBuilder.Entity<ContractParty>()
                .HasOne(cp => cp.Contract)
                .WithMany(c => c.Parties)
                .HasForeignKey(cp => cp.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contract -> Parent Contract (Self-referencing)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.ParentContract)
                .WithMany(c => c.ChildContracts)
                .HasForeignKey(c => c.ParentContractId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for performance
            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.ContractNumber)
                .IsUnique();

            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.Status);

            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.StartDate);

            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.EndDate);

            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.TenantId);

            modelBuilder.Entity<ContractType>()
                .HasIndex(ct => new { ct.TenantId, ct.Name })
                .IsUnique();

            modelBuilder.Entity<ContractParty>()
                .HasIndex(cp => cp.ContractId);
        }

        /// <summary>
        /// Seed initial data
        /// </summary>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default contract types (these will be created for each tenant)
            var defaultContractTypes = new[]
            {
                new ContractType
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Employment Agreement",
                    Description = "Employment contracts and job agreements",
                    Color = "#28a745",
                    Icon = "user-tie",
                    DefaultDurationDays = 365,
                    RequiresApproval = true,
                    SupportsRenewal = true,
                    DefaultReminderDays = 30,
                    SortOrder = 1,
                    TenantId = Guid.Empty, // Will be set per tenant
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty
                },
                new ContractType
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Service Agreement",
                    Description = "Service contracts and vendor agreements",
                    Color = "#007bff",
                    Icon = "handshake",
                    DefaultDurationDays = 365,
                    RequiresApproval = true,
                    SupportsRenewal = true,
                    DefaultReminderDays = 60,
                    SortOrder = 2,
                    TenantId = Guid.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty
                },
                new ContractType
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Non-Disclosure Agreement",
                    Description = "Confidentiality and non-disclosure agreements",
                    Color = "#ffc107",
                    Icon = "eye-slash",
                    DefaultDurationDays = 1095, // 3 years
                    RequiresApproval = false,
                    SupportsRenewal = false,
                    DefaultReminderDays = 90,
                    SortOrder = 3,
                    TenantId = Guid.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty
                },
                new ContractType
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Lease Agreement",
                    Description = "Property and equipment lease contracts",
                    Color = "#17a2b8",
                    Icon = "building",
                    DefaultDurationDays = 365,
                    RequiresApproval = true,
                    SupportsRenewal = true,
                    DefaultReminderDays = 45,
                    SortOrder = 4,
                    TenantId = Guid.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty
                },
                new ContractType
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Purchase Agreement",
                    Description = "Purchase orders and procurement contracts",
                    Color = "#dc3545",
                    Icon = "shopping-cart",
                    DefaultDurationDays = 90,
                    RequiresApproval = true,
                    SupportsRenewal = false,
                    DefaultReminderDays = 15,
                    SortOrder = 5,
                    TenantId = Guid.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty
                }
            };

            // Note: In a real application, you would seed this data per tenant
            // For now, we'll create these as templates that can be copied per tenant
            // modelBuilder.Entity<ContractType>().HasData(defaultContractTypes);
        }

        /// <summary>
        /// Override SaveChanges to automatically set audit fields and tenant context
        /// </summary>
        public override int SaveChanges()
        {
            SetAuditFields();
            return base.SaveChanges();
        }

        /// <summary>
        /// Override SaveChangesAsync to automatically set audit fields and tenant context
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Set audit fields for tracked entities
        /// </summary>
        private void SetAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && 
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            var currentTime = DateTime.UtcNow;
            var currentUserId = _tenantContext.IsAuthenticated ? _tenantContext.UserId : Guid.Empty;
            var currentTenantId = _tenantContext.IsAuthenticated ? _tenantContext.TenantId : Guid.Empty;

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                    entity.CreatedBy = currentUserId;
                    entity.TenantId = currentTenantId;
                }

                entity.UpdatedAt = currentTime;
                entity.UpdatedBy = currentUserId;

                // Ensure TenantId is always set correctly
                if (entity.TenantId == Guid.Empty && currentTenantId != Guid.Empty)
                {
                    entity.TenantId = currentTenantId;
                }
            }

            // Handle soft deletes
            var deletedEntries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseAuditEntity && e.State == EntityState.Deleted);

            foreach (var entry in deletedEntries)
            {
                var entity = (BaseAuditEntity)entry.Entity;
                
                // Convert hard delete to soft delete
                entry.State = EntityState.Modified;
                entity.IsDeleted = true;
                entity.DeletedAt = currentTime;
                entity.DeletedBy = currentUserId;
                entity.UpdatedAt = currentTime;
                entity.UpdatedBy = currentUserId;
            }
        }
    }
}