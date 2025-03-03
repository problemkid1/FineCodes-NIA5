using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class OpportunityContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactID",
                table: "Opportunities",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_ContactID",
                table: "Opportunities",
                column: "ContactID");

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

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_ContactID",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "ContactID",
                table: "Opportunities");
        }
    }
}
