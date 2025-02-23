using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberContacts_Contacts_ContactID",
                table: "MemberContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberContacts_Members_MemberID",
                table: "MemberContacts");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberContacts_Contacts_ContactID",
                table: "MemberContacts",
                column: "ContactID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberContacts_Members_MemberID",
                table: "MemberContacts",
                column: "MemberID",
                principalTable: "Members",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberContacts_Contacts_ContactID",
                table: "MemberContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberContacts_Members_MemberID",
                table: "MemberContacts");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberContacts_Contacts_ContactID",
                table: "MemberContacts",
                column: "ContactID",
                principalTable: "Contacts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberContacts_Members_MemberID",
                table: "MemberContacts",
                column: "MemberID",
                principalTable: "Members",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
