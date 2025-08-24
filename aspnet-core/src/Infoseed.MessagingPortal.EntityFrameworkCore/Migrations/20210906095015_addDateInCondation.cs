using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addDateInCondation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OrderOfferDateEnd",
                table: "OrderOffer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OrderOfferDateEndS",
                table: "OrderOffer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderOfferDateStart",
                table: "OrderOffer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OrderOfferDateStartS",
                table: "OrderOffer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderOfferDateEnd",
                table: "OrderOffer");

            migrationBuilder.DropColumn(
                name: "OrderOfferDateEndS",
                table: "OrderOffer");

            migrationBuilder.DropColumn(
                name: "OrderOfferDateStart",
                table: "OrderOffer");

            migrationBuilder.DropColumn(
                name: "OrderOfferDateStartS",
                table: "OrderOffer");
        }
    }
}
