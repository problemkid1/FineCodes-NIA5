using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class RemoveOpportunityConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Contacts_ContactID",
                table: "Opportunities");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Contacts_ContactID",
                table: "Opportunities",
                column: "ContactID",
                principalTable: "Contacts",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Contacts_ContactID",
                table: "Opportunities");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Contacts_ContactID",
                table: "Opportunities",
                column: "ContactID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
