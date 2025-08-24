using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Regenerated_TemplateMessage2372 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageAttachment",
                table: "TemplateMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "MessageAttachment",
                table: "TemplateMessages",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
