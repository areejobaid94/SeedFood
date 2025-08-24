using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addBookingToTeanant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InsideNumber",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBooking",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OutsideNumber",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsideNumber",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "IsBooking",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "OutsideNumber",
                table: "AbpTenants");
        }
    }
}
