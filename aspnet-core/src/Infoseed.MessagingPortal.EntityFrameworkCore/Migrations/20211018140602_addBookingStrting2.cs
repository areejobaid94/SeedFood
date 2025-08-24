using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addBookingStrting2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Booking",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Booking",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPhone",
                table: "Booking",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "UserPhone",
                table: "Booking");
        }
    }
}
