using ContractService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractService.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the ContractParty entity
    /// </summary>
    public class ContractPartyConfiguration : IEntityTypeConfiguration<ContractParty>
    {
        public void Configure(EntityTypeBuilder<ContractParty> builder)
        {
            // Table configuration
            builder.ToTable("contract_parties");
            builder.HasKey(cp => cp.Id);

            // Properties configuration
            builder.Property(cp => cp.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(cp => cp.ContractId)
                .HasColumnName("contract_id")
                .IsRequired();

            builder.Property(cp => cp.PartyType)
                .HasColumnName("party_type")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(cp => cp.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(cp => cp.LegalName)
                .HasColumnName("legal_name")
                .HasMaxLength(200);

            builder.Property(cp => cp.ContactPersonName)
                .HasColumnName("contact_person_name")
                .HasMaxLength(100);

            builder.Property(cp => cp.ContactPersonTitle)
                .HasColumnName("contact_person_title")
                .HasMaxLength(100);

            builder.Property(cp => cp.Email)
                .HasColumnName("email")
                .HasMaxLength(255);

            builder.Property(cp => cp.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20);

            builder.Property(cp => cp.AddressLine1)
                .HasColumnName("address_line1")
                .HasMaxLength(200);

            builder.Property(cp => cp.AddressLine2)
                .HasColumnName("address_line2")
                .HasMaxLength(200);

            builder.Property(cp => cp.City)
                .HasColumnName("city")
                .HasMaxLength(100);

            builder.Property(cp => cp.State)
                .HasColumnName("state")
                .HasMaxLength(100);

            builder.Property(cp => cp.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(20);

            builder.Property(cp => cp.Country)
                .HasColumnName("country")
                .HasMaxLength(100);

            builder.Property(cp => cp.TaxId)
                .HasColumnName("tax_id")
                .HasMaxLength(50);

            builder.Property(cp => cp.RegistrationNumber)
                .HasColumnName("registration_number")
                .HasMaxLength(50);

            builder.Property(cp => cp.Website)
                .HasColumnName("website")
                .HasMaxLength(255);

            builder.Property(cp => cp.RequiresSignature)
                .HasColumnName("requires_signature")
                .HasDefaultValue(true);

            builder.Property(cp => cp.SignedDate)
                .HasColumnName("signed_date")
                .HasColumnType("timestamp with time zone");

            builder.Property(cp => cp.SignedByName)
                .HasColumnName("signed_by_name")
                .HasMaxLength(100);

            builder.Property(cp => cp.SignedByTitle)
                .HasColumnName("signed_by_title")
                .HasMaxLength(100);

            builder.Property(cp => cp.SignatureData)
                .HasColumnName("signature_data")
                .HasColumnType("text");

            builder.Property(cp => cp.SignatureIpAddress)
                .HasColumnName("signature_ip_address")
                .HasMaxLength(45);

            builder.Property(cp => cp.Notes)
                .HasColumnName("notes")
                .HasMaxLength(1000);

            builder.Property(cp => cp.SortOrder)
                .HasColumnName("sort_order")
                .HasDefaultValue(0);

            // Base entity properties
            builder.Property(cp => cp.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(cp => cp.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(cp => cp.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(cp => cp.UpdatedBy)
                .HasColumnName("updated_by");

            builder.Property(cp => cp.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            // Indexes
            builder.HasIndex(cp => cp.ContractId)
                .HasDatabaseName("ix_contract_parties_contract_id");

            builder.HasIndex(cp => cp.PartyType)
                .HasDatabaseName("ix_contract_parties_party_type");

            builder.HasIndex(cp => cp.TenantId)
                .HasDatabaseName("ix_contract_parties_tenant_id");

            builder.HasIndex(cp => cp.Email)
                .HasDatabaseName("ix_contract_parties_email");

            builder.HasIndex(cp => new { cp.ContractId, cp.PartyType })
                .HasDatabaseName("ix_contract_parties_contract_party_type");

            builder.HasIndex(cp => new { cp.TenantId, cp.Name })
                .HasDatabaseName("ix_contract_parties_tenant_name");
        }
    }
}