using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addBookingStrting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumberPersonsBreakfastString",
                table: "Booking",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumberPersonsDinnerString",
                table: "Booking",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumberPersonsLunchString",
                table: "Booking",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberPersonsBreakfastString",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "NumberPersonsDinnerString",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "NumberPersonsLunchString",
                table: "Booking");
        }
    }
}
