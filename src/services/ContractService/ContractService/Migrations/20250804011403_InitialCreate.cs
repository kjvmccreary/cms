using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contract_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#007bff"),
                    icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    default_duration_days = table.Column<int>(type: "integer", nullable: true),
                    requires_approval = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    supports_renewal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    default_reminder_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    template = table.Column<string>(type: "text", nullable: true),
                    custom_fields = table.Column<string>(type: "jsonb", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    contract_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    signed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    value = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    billing_frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    auto_renewal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    renewal_reminder_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    auto_renewal_duration_days = table.Column<int>(type: "integer", nullable: true),
                    terms = table.Column<string>(type: "text", nullable: true),
                    internal_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    custom_fields = table.Column<string>(type: "jsonb", nullable: true),
                    parent_contract_id = table.Column<Guid>(type: "uuid", nullable: true),
                    department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    project_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approval_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    last_status_change_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_status_changed_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status_change_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    notifications_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    document_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    document_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    document_content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    document_size = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.id);
                    table.ForeignKey(
                        name: "FK_contracts_contract_types_contract_type_id",
                        column: x => x.contract_type_id,
                        principalTable: "contract_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contracts_contracts_parent_contract_id",
                        column: x => x.parent_contract_id,
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "contract_parties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    party_type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    legal_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    contact_person_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    contact_person_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address_line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address_line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tax_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    registration_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    requires_signature = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    signed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    signed_by_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    signed_by_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    signature_data = table.Column<string>(type: "text", nullable: true),
                    signature_ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract_parties", x => x.id);
                    table.ForeignKey(
                        name: "FK_contract_parties_contracts_contract_id",
                        column: x => x.contract_id,
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contract_parties_contract_id",
                table: "contract_parties",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_parties_contract_party_type",
                table: "contract_parties",
                columns: new[] { "contract_id", "party_type" });

            migrationBuilder.CreateIndex(
                name: "ix_contract_parties_email",
                table: "contract_parties",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_contract_parties_party_type",
                table: "contract_parties",
                column: "party_type");

            migrationBuilder.CreateIndex(
                name: "ix_contract_parties_tenant_id",
                table: "contract_parties",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_parties_tenant_name",
                table: "contract_parties",
                columns: new[] { "tenant_id", "name" });

            migrationBuilder.CreateIndex(
                name: "ix_contract_types_is_active",
                table: "contract_types",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_contract_types_sort_order",
                table: "contract_types",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "ix_contract_types_tenant_id",
                table: "contract_types",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_types_tenant_name",
                table: "contract_types",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contracts_parent_contract_id",
                table: "contracts",
                column: "parent_contract_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_contract_number",
                table: "contracts",
                column: "contract_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contracts_contract_type_id",
                table: "contracts",
                column: "contract_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_end_date",
                table: "contracts",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_owner_id",
                table: "contracts",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_start_date",
                table: "contracts",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_status",
                table: "contracts",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_end_date",
                table: "contracts",
                columns: new[] { "tenant_id", "end_date" });

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_id",
                table: "contracts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_status",
                table: "contracts",
                columns: new[] { "tenant_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contract_parties");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "contract_types");
        }
    }
}
