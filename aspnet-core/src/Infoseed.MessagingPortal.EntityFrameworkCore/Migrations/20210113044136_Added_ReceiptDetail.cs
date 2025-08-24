using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_ReceiptDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptDetails",
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
                    BillingNumber = table.Column<string>(maxLength: 256, nullable: true),
                    BillDateFrom = table.Column<DateTime>(nullable: false),
                    BillDateTo = table.Column<DateTime>(nullable: false),
                    ServiceName = table.Column<string>(maxLength: 1024, nullable: false),
                    BillAmount = table.Column<decimal>(nullable: true),
                    OpenAmount = table.Column<decimal>(nullable: true),
                    CurrencyName = table.Column<string>(maxLength: 256, nullable: false),
                    AccountBillingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_AccountBillings_AccountBillingId",
                        column: x => x.AccountBillingId,
                        principalTable: "AccountBillings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_AccountBillingId",
                table: "ReceiptDetails",
                column: "AccountBillingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptDetails");
        }
    }
}
