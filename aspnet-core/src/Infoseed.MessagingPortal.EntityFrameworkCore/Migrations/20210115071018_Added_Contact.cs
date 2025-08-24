using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_Contact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    AvatarUrl = table.Column<string>(maxLength: 512, nullable: true),
                    DisplayName = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 512, nullable: true),
                    SunshineAppID = table.Column<string>(maxLength: 512, nullable: true),
                    IsLockedByAgent = table.Column<string>(maxLength: 512, nullable: true),
                    LockedByAgentName = table.Column<string>(maxLength: 512, nullable: true),
                    IsOpen = table.Column<bool>(nullable: false),
                    Website = table.Column<string>(maxLength: 512, nullable: true),
                    EmailAddress = table.Column<string>(maxLength: 512, nullable: true),
                    Description = table.Column<string>(maxLength: 5012, nullable: true),
                    ChatStatuseId = table.Column<int>(nullable: true),
                    ContactStatuseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_ChatStatuses_ChatStatuseId",
                        column: x => x.ChatStatuseId,
                        principalTable: "ChatStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contacts_ContactStatuses_ContactStatuseId",
                        column: x => x.ContactStatuseId,
                        principalTable: "ContactStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ChatStatuseId",
                table: "Contacts",
                column: "ChatStatuseId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactStatuseId",
                table: "Contacts",
                column: "ContactStatuseId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_TenantId",
                table: "Contacts",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
