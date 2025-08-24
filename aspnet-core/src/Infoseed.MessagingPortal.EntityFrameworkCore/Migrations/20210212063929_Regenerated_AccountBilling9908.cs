using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Regenerated_AccountBilling9908 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingId",
                table: "AccountBillings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountBillings_BillingId",
                table: "AccountBillings",
                column: "BillingId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBillings_Billings_BillingId",
                table: "AccountBillings",
                column: "BillingId",
                principalTable: "Billings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBillings_Billings_BillingId",
                table: "AccountBillings");

            migrationBuilder.DropIndex(
                name: "IX_AccountBillings_BillingId",
                table: "AccountBillings");

            migrationBuilder.DropColumn(
                name: "BillingId",
                table: "AccountBillings");
        }
    }
}
