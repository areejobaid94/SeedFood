using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addAreaNameEnglish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaCoordinateEnglish",
                table: "Areas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AreaNameEnglish",
                table: "Areas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaCoordinateEnglish",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "AreaNameEnglish",
                table: "Areas");
        }
    }
}
