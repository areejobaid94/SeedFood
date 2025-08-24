using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addisPersanteg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrentBookingNumber",
                table: "TenantInformations",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "isPersentageDiscount",
                table: "OrderOffer",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBookingNumber",
                table: "TenantInformations");

            migrationBuilder.DropColumn(
                name: "isPersentageDiscount",
                table: "OrderOffer");
        }
    }
}
