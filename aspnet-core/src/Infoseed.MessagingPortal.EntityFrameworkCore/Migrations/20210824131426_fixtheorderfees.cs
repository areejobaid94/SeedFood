using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class fixtheorderfees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderOfferStart",
                table: "OrderOffer",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderOfferEnd",
                table: "OrderOffer",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderOfferEndS",
                table: "OrderOffer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderOfferStartS",
                table: "OrderOffer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderOfferEndS",
                table: "OrderOffer");

            migrationBuilder.DropColumn(
                name: "OrderOfferStartS",
                table: "OrderOffer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderOfferStart",
                table: "OrderOffer",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderOfferEnd",
                table: "OrderOffer",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));
        }
    }
}
