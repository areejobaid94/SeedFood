using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addEnglishToMenu2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuDescriptionArabic",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuNameArabic",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "CategoryNamesArabic",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemDescriptionArabic",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemNameArabic",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "NameArabic",
                table: "ItemCategorys");

            migrationBuilder.DropColumn(
                name: "NameArabic",
                table: "ItemAdditions");

            migrationBuilder.AddColumn<string>(
                name: "MenuDescription",
                table: "Menus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MenuName",
                table: "Menus",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoryNames",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "Items",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ItemCategorys",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ItemAdditions",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuDescription",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuName",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "CategoryNames",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ItemCategorys");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ItemAdditions");

            migrationBuilder.AddColumn<string>(
                name: "MenuDescriptionArabic",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MenuNameArabic",
                table: "Menus",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoryNamesArabic",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescriptionArabic",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemNameArabic",
                table: "Items",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameArabic",
                table: "ItemCategorys",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameArabic",
                table: "ItemAdditions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
