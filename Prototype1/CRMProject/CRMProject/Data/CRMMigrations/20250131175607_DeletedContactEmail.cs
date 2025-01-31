using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class DeletedContactEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactEmails");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmailAddress",
                table: "Contacts",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactEmailType",
                table: "Contacts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactEmailAddress",
                table: "Contacts",
                column: "ContactEmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contacts_ContactEmailAddress",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ContactEmailAddress",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ContactEmailType",
                table: "Contacts");

            migrationBuilder.CreateTable(
                name: "ContactEmails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContactID = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    EmailType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactEmails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContactEmails_Contacts_ContactID",
                        column: x => x.ContactID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactEmails_ContactID",
                table: "ContactEmails",
                column: "ContactID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactEmails_EmailAddress",
                table: "ContactEmails",
                column: "EmailAddress",
                unique: true);
        }
    }
}
