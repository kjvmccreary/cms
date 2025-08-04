using ContractService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractService.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the ContractType entity
    /// </summary>
    public class ContractTypeConfiguration : IEntityTypeConfiguration<ContractType>
    {
        public void Configure(EntityTypeBuilder<ContractType> builder)
        {
            // Table configuration
            builder.ToTable("contract_types");
            builder.HasKey(ct => ct.Id);

            // Properties configuration
            builder.Property(ct => ct.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(ct => ct.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(ct => ct.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            builder.Property(ct => ct.Color)
                .HasColumnName("color")
                .HasMaxLength(7)
                .HasDefaultValue("#007bff");

            builder.Property(ct => ct.Icon)
                .HasColumnName("icon")
                .HasMaxLength(50);

            builder.Property(ct => ct.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            builder.Property(ct => ct.DefaultDurationDays)
                .HasColumnName("default_duration_days");

            builder.Property(ct => ct.RequiresApproval)
                .HasColumnName("requires_approval")
                .HasDefaultValue(true);

            builder.Property(ct => ct.SupportsRenewal)
                .HasColumnName("supports_renewal")
                .HasDefaultValue(true);

            builder.Property(ct => ct.DefaultReminderDays)
                .HasColumnName("default_reminder_days")
                .HasDefaultValue(30);

            builder.Property(ct => ct.Template)
                .HasColumnName("template")
                .HasColumnType("text");

            builder.Property(ct => ct.CustomFields)
                .HasColumnName("custom_fields")
                .HasColumnType("jsonb");

            builder.Property(ct => ct.SortOrder)
                .HasColumnName("sort_order")
                .HasDefaultValue(0);

            // Base entity properties
            builder.Property(ct => ct.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(ct => ct.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(ct => ct.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(ct => ct.UpdatedBy)
                .HasColumnName("updated_by");

            builder.Property(ct => ct.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            // Indexes
            builder.HasIndex(ct => new { ct.TenantId, ct.Name })
                .HasDatabaseName("ix_contract_types_tenant_name")
                .IsUnique();

            builder.HasIndex(ct => ct.TenantId)
                .HasDatabaseName("ix_contract_types_tenant_id");

            builder.HasIndex(ct => ct.IsActive)
                .HasDatabaseName("ix_contract_types_is_active");

            builder.HasIndex(ct => ct.SortOrder)
                .HasDatabaseName("ix_contract_types_sort_order");
        }
    }
}