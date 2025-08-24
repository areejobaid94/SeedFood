using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_AccountBilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountBillings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    BillID = table.Column<string>(maxLength: 256, nullable: false),
                    BillDateFrom = table.Column<DateTime>(nullable: false),
                    BillDateTo = table.Column<DateTime>(nullable: false),
                    OpenAmount = table.Column<decimal>(nullable: true),
                    BillAmount = table.Column<decimal>(nullable: true),
                    InfoSeedServiceId = table.Column<int>(nullable: true),
                    ServiceTypeId = table.Column<int>(nullable: true),
                    CurrencyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBillings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountBillings_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountBillings_InfoSeedServices_InfoSeedServiceId",
                        column: x => x.InfoSeedServiceId,
                        principalTable: "InfoSeedServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountBillings_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountBillings_CurrencyId",
                table: "AccountBillings",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBillings_InfoSeedServiceId",
                table: "AccountBillings",
                column: "InfoSeedServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBillings_ServiceTypeId",
                table: "AccountBillings",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBillings_TenantId",
                table: "AccountBillings",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBillings");
        }
    }
}
