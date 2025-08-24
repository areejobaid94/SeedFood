using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addBackGroundImagMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageBgUri",
                table: "Menus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bgImag",
                table: "ItemCategorys",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "logoImag",
                table: "ItemCategorys",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageBgUri",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "bgImag",
                table: "ItemCategorys");

            migrationBuilder.DropColumn(
                name: "logoImag",
                table: "ItemCategorys");
        }
    }
}
