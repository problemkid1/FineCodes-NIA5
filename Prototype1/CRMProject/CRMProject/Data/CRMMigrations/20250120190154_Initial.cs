using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMProject.Data.CRMMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContactTitleRole = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ContactWebsite = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ContactInteractions = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ContactNotes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Industries",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IndustryName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    IndustryNAICSCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    IndustryDescription = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Industries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    MemberSize = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberAccountsPayableEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    MemberStartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MemberEndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MemberNotes = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MembershipTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MembershipTypeName = table.Column<int>(type: "INTEGER", nullable: false),
                    MembershipTypeDescription = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MembershipTypeFee = table.Column<double>(type: "REAL", nullable: false),
                    MembershipTypeBenefits = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OpportunityName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    OpportunityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    OpportunityPriority = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    OpportunityAction = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    OpportunityContact = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    OpportunityAccount = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    OpportunityLastContactDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OpportunityInteractions = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContactEmails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmailType = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContactID = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressLine1 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AddressLine2 = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    AddressCity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Province = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: false),
                    AddressType = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Addresses_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cancellations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CancellationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CancellationReason = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CancellationNotes = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancellations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cancellations_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberContacts",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactID = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberContactRelationshipType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberContacts", x => new { x.MemberID, x.ContactID });
                    table.ForeignKey(
                        name: "FK_MemberContacts_Contacts_ContactID",
                        column: x => x.ContactID,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberContacts_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberIndustries",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false),
                    IndustryID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberIndustries", x => new { x.MemberID, x.IndustryID });
                    table.ForeignKey(
                        name: "FK_MemberIndustries_Industries_IndustryID",
                        column: x => x.IndustryID,
                        principalTable: "Industries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberIndustries_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberMembershipTypes",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "INTEGER", nullable: false),
                    MembershipTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberMembershipTypes", x => new { x.MemberID, x.MembershipTypeID });
                    table.ForeignKey(
                        name: "FK_MemberMembershipTypes_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberMembershipTypes_MembershipTypes_MembershipTypeID",
                        column: x => x.MembershipTypeID,
                        principalTable: "MembershipTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_MemberID",
                table: "Addresses",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellations_MemberID",
                table: "Cancellations",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactEmails_ContactID",
                table: "ContactEmails",
                column: "ContactID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactEmails_EmailAddress",
                table: "ContactEmails",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Industries_IndustryNAICSCode",
                table: "Industries",
                column: "IndustryNAICSCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Industries_IndustryName",
                table: "Industries",
                column: "IndustryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberContacts_ContactID",
                table: "MemberContacts",
                column: "ContactID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberIndustries_IndustryID",
                table: "MemberIndustries",
                column: "IndustryID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMembershipTypes_MembershipTypeID",
                table: "MemberMembershipTypes",
                column: "MembershipTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberName",
                table: "Members",
                column: "MemberName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipTypes_MembershipTypeName",
                table: "MembershipTypes",
                column: "MembershipTypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_OpportunityAccount",
                table: "Opportunities",
                column: "OpportunityAccount",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_OpportunityName",
                table: "Opportunities",
                column: "OpportunityName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Cancellations");

            migrationBuilder.DropTable(
                name: "ContactEmails");

            migrationBuilder.DropTable(
                name: "MemberContacts");

            migrationBuilder.DropTable(
                name: "MemberIndustries");

            migrationBuilder.DropTable(
                name: "MemberMembershipTypes");

            migrationBuilder.DropTable(
                name: "Opportunities");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Industries");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "MembershipTypes");
        }
    }
}
