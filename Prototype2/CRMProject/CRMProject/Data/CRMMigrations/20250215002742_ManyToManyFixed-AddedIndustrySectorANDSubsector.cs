using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class ManyToManyFixedAddedIndustrySectorANDSubsector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Industries_IndustryName",
                table: "Industries");

            migrationBuilder.RenameColumn(
                name: "IndustryName",
                table: "Industries",
                newName: "IndustrySubsector");

            migrationBuilder.RenameColumn(
                name: "IndustryDescription",
                table: "Industries",
                newName: "IndustrySector");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IndustrySubsector",
                table: "Industries",
                newName: "IndustryName");

            migrationBuilder.RenameColumn(
                name: "IndustrySector",
                table: "Industries",
                newName: "IndustryDescription");

            migrationBuilder.CreateIndex(
                name: "IX_Industries_IndustryName",
                table: "Industries",
                column: "IndustryName",
                unique: true);
        }
    }
}
