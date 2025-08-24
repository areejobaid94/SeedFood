using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class updatetenants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "AppBinaryObjects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "AppBinaryObjects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmoochAPIKeyID",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmoochAppID",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmoochSecretKey",
                table: "AbpTenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "AppBinaryObjects");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "AppBinaryObjects");

            migrationBuilder.DropColumn(
                name: "SmoochAPIKeyID",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "SmoochAppID",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "SmoochSecretKey",
                table: "AbpTenants");
        }
    }
}
