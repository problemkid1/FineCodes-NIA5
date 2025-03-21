using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class MtoMOpportunityContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "MemberContactRelationshipType",
                table: "MemberContacts");

            migrationBuilder.CreateTable(
                name: "OpportunityContacts",
                columns: table => new
                {
                    OpportunityID = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityContacts", x => new { x.OpportunityID, x.ContactID });
                    table.ForeignKey(
                        name: "FK_OpportunityContacts_Contacts_ContactID",
                        column: x => x.ContactID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunityContacts_Opportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "Opportunities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityContacts_ContactID",
                table: "OpportunityContacts",
                column: "ContactID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpportunityContacts");

            migrationBuilder.AddColumn<int>(
                name: "ContactID",
                table: "Opportunities",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberContactRelationshipType",
                table: "MemberContacts",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

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
    }
}
