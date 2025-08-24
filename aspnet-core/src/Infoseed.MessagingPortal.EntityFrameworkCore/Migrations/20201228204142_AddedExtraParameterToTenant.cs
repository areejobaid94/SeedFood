using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddedExtraParameterToTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommercialName",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerLegalName",
                table: "AbpTenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommercialName",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CustomerLegalName",
                table: "AbpTenants");
        }
    }
}
