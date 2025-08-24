using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_TemplateMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    TemplateMessageName = table.Column<string>(maxLength: 256, nullable: false),
                    MessageTest = table.Column<string>(maxLength: 1024, nullable: false),
                    MessageAttachment = table.Column<byte>(nullable: false),
                    TemplateMessagePurposeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateMessages_TemplateMessagePurposes_TemplateMessagePurposeId",
                        column: x => x.TemplateMessagePurposeId,
                        principalTable: "TemplateMessagePurposes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateMessages_TemplateMessagePurposeId",
                table: "TemplateMessages",
                column: "TemplateMessagePurposeId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateMessages_TenantId",
                table: "TemplateMessages",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateMessages");
        }
    }
}
