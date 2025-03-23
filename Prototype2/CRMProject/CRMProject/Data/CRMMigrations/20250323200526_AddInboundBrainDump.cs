using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class AddInboundBrainDump : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrainDumps",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Activity = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Assignee = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    BrainDumpStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    BrainDumpTerm = table.Column<int>(type: "INTEGER", nullable: true),
                    BrainDumpNotes = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrainDumps", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InboundInitiatives",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Initiative = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    InboundInitiativeNotes = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundInitiatives", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrainDumps");

            migrationBuilder.DropTable(
                name: "InboundInitiatives");
        }
    }
}
