using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addEnglishToMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MenuDescriptionEnglish",
                table: "Menus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MenuNameArabic",
                table: "Menus",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MenuNameEnglish",
                table: "Menus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryNamesArabic",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryNamesEnglish",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescriptionArabic",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescriptionEnglish",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemNameArabic",
                table: "Items",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ItemNameEnglish",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameArabic",
                table: "ItemCategorys",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEnglish",
                table: "ItemCategorys",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameArabic",
                table: "ItemAdditions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEnglish",
                table: "ItemAdditions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuDescriptionArabic",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuDescriptionEnglish",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuNameArabic",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuNameEnglish",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "CategoryNamesArabic",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CategoryNamesEnglish",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemDescriptionArabic",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemDescriptionEnglish",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemNameArabic",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemNameEnglish",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "NameArabic",
                table: "ItemCategorys");

            migrationBuilder.DropColumn(
                name: "NameEnglish",
                table: "ItemCategorys");

            migrationBuilder.DropColumn(
                name: "NameArabic",
                table: "ItemAdditions");

            migrationBuilder.DropColumn(
                name: "NameEnglish",
                table: "ItemAdditions");

            migrationBuilder.AddColumn<string>(
                name: "MenuDescription",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MenuName",
                table: "Menus",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoryNames",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "Items",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ItemCategorys",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ItemAdditions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
