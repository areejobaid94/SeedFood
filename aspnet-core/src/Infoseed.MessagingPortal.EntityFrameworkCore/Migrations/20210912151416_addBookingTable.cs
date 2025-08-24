using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addBookingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    TableNumber = table.Column<string>(nullable: true),
                    BookTableDate = table.Column<DateTime>(nullable: false),
                    IsBreakfast = table.Column<bool>(nullable: false),
                    NumberPersonsBreakfast = table.Column<int>(nullable: false),
                    BreakfastUserId = table.Column<string>(nullable: true),
                    BreakfastUserName = table.Column<string>(nullable: true),
                    BreakfastUserPhone = table.Column<string>(nullable: true),
                    IsLunch = table.Column<bool>(nullable: false),
                    NumberPersonsLunch = table.Column<int>(nullable: false),
                    LunchUserId = table.Column<string>(nullable: true),
                    LunchUserName = table.Column<string>(nullable: true),
                    LunchUserPhone = table.Column<string>(nullable: true),
                    IsDinner = table.Column<bool>(nullable: false),
                    NumberPersonsDinner = table.Column<int>(nullable: false),
                    DinnerUserId = table.Column<string>(nullable: true),
                    DinnerUserName = table.Column<string>(nullable: true),
                    DinnerUserPhone = table.Column<string>(nullable: true),
                    bookingTypeEunm = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");
        }
    }
}
