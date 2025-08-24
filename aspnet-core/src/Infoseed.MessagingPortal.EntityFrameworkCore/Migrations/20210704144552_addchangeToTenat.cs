using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addchangeToTenat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantType",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "TenantType",
                table: "AbpTenants");
        }
    }
}
