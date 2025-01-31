using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class OneAddressPerMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_MemberID",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_MemberID",
                table: "Addresses",
                column: "MemberID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_MemberID",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_MemberID",
                table: "Addresses",
                column: "MemberID");
        }
    }
}
