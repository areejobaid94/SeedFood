using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddResturantTypeToMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestaurantsType",
                table: "Menus",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestaurantsType",
                table: "Menus");
        }
    }
}
