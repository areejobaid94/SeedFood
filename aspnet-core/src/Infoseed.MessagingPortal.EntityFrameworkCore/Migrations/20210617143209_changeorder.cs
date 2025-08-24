using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class changeorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationID",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocationID",
                table: "Branches",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LocationID",
                table: "Branches");
        }
    }
}
