using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddBell1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BellSrc",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBellContinues",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBellOn",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BellSrc",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "IsBellContinues",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "IsBellOn",
                table: "AbpTenants");
        }
    }
}
