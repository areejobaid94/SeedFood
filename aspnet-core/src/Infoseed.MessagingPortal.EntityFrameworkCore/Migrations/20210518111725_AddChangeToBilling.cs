using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddChangeToBilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayed",
                table: "Billings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantServiceId",
                table: "Billings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Billings_TenantServices_CurrencyId",
                table: "Billings",
                column: "CurrencyId",
                principalTable: "TenantServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Billings_TenantServices_CurrencyId",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "IsPayed",
                table: "Billings");

            migrationBuilder.DropColumn(
                name: "TenantServiceId",
                table: "Billings");
        }
    }
}
