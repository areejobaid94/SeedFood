using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddSKUNumberInItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "ItemAdditions",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryCost",
                table: "Branches",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "BranchID",
                table: "Areas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SKU",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "ItemAdditions");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "Areas");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeliveryCost",
                table: "Branches",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }
    }
}
