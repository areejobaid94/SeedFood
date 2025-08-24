using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addPreOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPreOrder",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SelectDay",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectTime",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreOrder",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SelectDay",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SelectTime",
                table: "Orders");
        }
    }
}
