using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddResturantTypeToMenu2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuType",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MenuType",
                table: "ItemCategorys",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "ItemCategorys",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MenuType",
                table: "ItemAdditions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuType",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MenuType",
                table: "ItemCategorys");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "ItemCategorys");

            migrationBuilder.DropColumn(
                name: "MenuType",
                table: "ItemAdditions");
        }
    }
}
