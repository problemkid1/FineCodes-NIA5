using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class updatedDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberAccountsPayableEmail",
                table: "Members",
                column: "MemberAccountsPayableEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AddressLine1_AddressCity_Province_PostalCode",
                table: "Addresses",
                columns: new[] { "AddressLine1", "AddressCity", "Province", "PostalCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_MemberAccountsPayableEmail",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_AddressLine1_AddressCity_Province_PostalCode",
                table: "Addresses");
        }
    }
}
