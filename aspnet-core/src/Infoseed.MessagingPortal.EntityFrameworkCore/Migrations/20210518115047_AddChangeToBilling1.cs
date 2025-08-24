using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddChangeToBilling1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Billings_TenantServices_CurrencyId",
                table: "Billings");

            migrationBuilder.CreateIndex(
                name: "IX_Billings_TenantServiceId",
                table: "Billings",
                column: "TenantServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Billings_TenantServices_TenantServiceId",
                table: "Billings",
                column: "TenantServiceId",
                principalTable: "TenantServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Billings_TenantServices_TenantServiceId",
                table: "Billings");

            migrationBuilder.DropIndex(
                name: "IX_Billings_TenantServiceId",
                table: "Billings");

            migrationBuilder.AddForeignKey(
                name: "FK_Billings_TenantServices_CurrencyId",
                table: "Billings",
                column: "CurrencyId",
                principalTable: "TenantServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
