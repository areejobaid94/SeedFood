using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class UpdateServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeesPerTransaction",
                table: "InfoSeedServices",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeesPerTransaction",
                table: "InfoSeedServices");
        }
    }
}
