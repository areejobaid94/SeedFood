using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_Deal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    DealName = table.Column<string>(nullable: true),
                    ARR = table.Column<string>(nullable: true),
                    OrderFees = table.Column<string>(nullable: true),
                    CloseDate = table.Column<DateTime>(nullable: false),
                    DealAge = table.Column<string>(nullable: true),
                    DealStatusId = table.Column<int>(nullable: false),
                    DealTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deals_DealStatuses_DealStatusId",
                        column: x => x.DealStatusId,
                        principalTable: "DealStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deals_DealTypes_DealTypeId",
                        column: x => x.DealTypeId,
                        principalTable: "DealTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_DealStatusId",
                table: "Deals",
                column: "DealStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_DealTypeId",
                table: "Deals",
                column: "DealTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_TenantId",
                table: "Deals",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deals");
        }
    }
}
