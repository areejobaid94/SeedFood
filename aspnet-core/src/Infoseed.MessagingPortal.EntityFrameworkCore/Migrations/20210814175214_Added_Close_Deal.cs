using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_Close_Deal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Close_Deals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerCommercialNameArabic = table.Column<string>(nullable: true),
                    ARR = table.Column<string>(nullable: true),
                    OrderFees = table.Column<string>(nullable: true),
                    ContrevelDate = table.Column<DateTime>(nullable: false),
                    FirstPay = table.Column<string>(nullable: true),
                    SecondPay = table.Column<string>(nullable: true),
                    AMClosed = table.Column<string>(nullable: true),
                    CloseDealStatusId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Close_Deals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Close_Deals_CloseDealStatuses_CloseDealStatusId",
                        column: x => x.CloseDealStatusId,
                        principalTable: "CloseDealStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Close_Deals_CloseDealStatusId",
                table: "Close_Deals",
                column: "CloseDealStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Close_Deals");
        }
    }
}
