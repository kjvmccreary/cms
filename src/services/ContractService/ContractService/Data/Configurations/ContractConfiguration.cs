using ContractService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractService.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the Contract entity
    /// </summary>
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            // Table configuration
            builder.ToTable("contracts");
            builder.HasKey(c => c.Id);

            // Properties configuration
            builder.Property(c => c.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(c => c.ContractNumber)
                .HasColumnName("contract_number")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasColumnName("description")
                .HasMaxLength(2000);

            builder.Property(c => c.ContractTypeId)
                .HasColumnName("contract_type_id")
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnName("status")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.Priority)
                .HasColumnName("priority")
                .HasDefaultValue(3);

            builder.Property(c => c.StartDate)
                .HasColumnName("start_date")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(c => c.EndDate)
                .HasColumnName("end_date")
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.SignedDate)
                .HasColumnName("signed_date")
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.Value)
                .HasColumnName("value")
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3);

            builder.Property(c => c.BillingFrequency)
                .HasColumnName("billing_frequency")
                .HasMaxLength(20);

            builder.Property(c => c.AutoRenewal)
                .HasColumnName("auto_renewal")
                .HasDefaultValue(false);

            builder.Property(c => c.RenewalReminderDays)
                .HasColumnName("renewal_reminder_days")
                .HasDefaultValue(30);

            builder.Property(c => c.AutoRenewalDurationDays)
                .HasColumnName("auto_renewal_duration_days");

            builder.Property(c => c.Terms)
                .HasColumnName("terms")
                .HasColumnType("text");

            builder.Property(c => c.InternalNotes)
                .HasColumnName("internal_notes")
                .HasMaxLength(2000);

            builder.Property(c => c.Tags)
                .HasColumnName("tags")
                .HasMaxLength(500);

            builder.Property(c => c.CustomFields)
                .HasColumnName("custom_fields")
                .HasColumnType("jsonb");

            builder.Property(c => c.ParentContractId)
                .HasColumnName("parent_contract_id");

            builder.Property(c => c.Department)
                .HasColumnName("department")
                .HasMaxLength(100);

            builder.Property(c => c.ProjectCode)
                .HasColumnName("project_code")
                .HasMaxLength(50);

            builder.Property(c => c.OwnerId)
                .HasColumnName("owner_id");

            builder.Property(c => c.ApprovedById)
                .HasColumnName("approved_by_id");

            builder.Property(c => c.ApprovedDate)
                .HasColumnName("approved_date")
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.ApprovalNotes)
                .HasColumnName("approval_notes")
                .HasMaxLength(500);

            builder.Property(c => c.LastStatusChangeDate)
                .HasColumnName("last_status_change_date")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(c => c.LastStatusChangedById)
                .HasColumnName("last_status_changed_by_id");

            builder.Property(c => c.StatusChangeReason)
                .HasColumnName("status_change_reason")
                .HasMaxLength(500);

            builder.Property(c => c.NotificationsEnabled)
                .HasColumnName("notifications_enabled")
                .HasDefaultValue(true);

            builder.Property(c => c.DocumentPath)
                .HasColumnName("document_path")
                .HasMaxLength(500);

            builder.Property(c => c.DocumentFileName)
                .HasColumnName("document_file_name")
                .HasMaxLength(255);

            builder.Property(c => c.DocumentContentType)
                .HasColumnName("document_content_type")
                .HasMaxLength(100);

            builder.Property(c => c.DocumentSize)
                .HasColumnName("document_size");

            // Base entity properties
            builder.Property(c => c.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(c => c.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(c => c.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(c => c.UpdatedBy)
                .HasColumnName("updated_by");

            builder.Property(c => c.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            // Audit entity properties
            builder.Property(c => c.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            builder.Property(c => c.DeletedAt)
                .HasColumnName("deleted_at")
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.DeletedBy)
                .HasColumnName("deleted_by");

            // Indexes
            builder.HasIndex(c => c.ContractNumber)
                .HasDatabaseName("ix_contracts_contract_number")
                .IsUnique();

            builder.HasIndex(c => c.Status)
                .HasDatabaseName("ix_contracts_status");

            builder.HasIndex(c => c.StartDate)
                .HasDatabaseName("ix_contracts_start_date");

            builder.HasIndex(c => c.EndDate)
                .HasDatabaseName("ix_contracts_end_date");

            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("ix_contracts_tenant_id");

            builder.HasIndex(c => c.ContractTypeId)
                .HasDatabaseName("ix_contracts_contract_type_id");

            builder.HasIndex(c => c.OwnerId)
                .HasDatabaseName("ix_contracts_owner_id");

            builder.HasIndex(c => new { c.TenantId, c.Status })
                .HasDatabaseName("ix_contracts_tenant_status");

            builder.HasIndex(c => new { c.TenantId, c.EndDate })
                .HasDatabaseName("ix_contracts_tenant_end_date");

            // Note: Full text search index can be added via custom migrations if needed
            // builder.HasIndex(c => new { c.Title, c.Description, c.Tags })
            //     .HasMethod("gin")
            //     .HasDatabaseName("ix_contracts_fulltext");
        }
    }
}