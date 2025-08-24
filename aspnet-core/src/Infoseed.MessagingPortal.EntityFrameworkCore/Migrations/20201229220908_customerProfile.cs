using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class customerProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonEmail",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonName",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonPhoneNumber",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerStatusID",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalID",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AbpTenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ContactPersonEmail",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ContactPersonName",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "ContactPersonPhoneNumber",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "CustomerStatusID",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "LegalID",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AbpTenants");
        }
    }
}
