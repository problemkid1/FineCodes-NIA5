using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class AddedMemberWebsite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactWebsite",
                table: "Contacts");

            migrationBuilder.AddColumn<string>(
                name: "MemberWebsite",
                table: "Members",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberWebsite",
                table: "Members");

            migrationBuilder.AddColumn<string>(
                name: "ContactWebsite",
                table: "Contacts",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }
    }
}
