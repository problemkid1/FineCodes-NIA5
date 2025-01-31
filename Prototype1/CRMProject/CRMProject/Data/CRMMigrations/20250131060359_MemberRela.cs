using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class MemberRela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberThumbnails_MemberID",
                table: "MemberThumbnails");

            migrationBuilder.DropIndex(
                name: "IX_MemberPhotos_MemberID",
                table: "MemberPhotos");

            migrationBuilder.CreateIndex(
                name: "IX_MemberThumbnails_MemberID",
                table: "MemberThumbnails",
                column: "MemberID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberPhotos_MemberID",
                table: "MemberPhotos",
                column: "MemberID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberThumbnails_MemberID",
                table: "MemberThumbnails");

            migrationBuilder.DropIndex(
                name: "IX_MemberPhotos_MemberID",
                table: "MemberPhotos");

            migrationBuilder.CreateIndex(
                name: "IX_MemberThumbnails_MemberID",
                table: "MemberThumbnails",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberPhotos_MemberID",
                table: "MemberPhotos",
                column: "MemberID");
        }
    }
}
