using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addchangestoOrderDeiteal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromLocationDescribation",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromLongitudeLatitude",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderDescribation",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToLocationDescribation",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToLongitudeLatitude",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromLocationDescribation",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FromLongitudeLatitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderDescribation",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ToLocationDescribation",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ToLongitudeLatitude",
                table: "Orders");
        }
    }
}
