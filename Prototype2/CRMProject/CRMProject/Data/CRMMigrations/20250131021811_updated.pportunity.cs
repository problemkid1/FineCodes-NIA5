using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class updatedpportunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Opportunities_OpportunityAccount",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "OpportunityAccount",
                table: "Opportunities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpportunityAccount",
                table: "Opportunities",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_OpportunityAccount",
                table: "Opportunities",
                column: "OpportunityAccount",
                unique: true);
        }
    }
}
