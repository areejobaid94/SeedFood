using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddChangeToItems2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_MenuCategories_MenuCategoryId",
                table: "Menus");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Menus_MenuId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_MenuId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Menus_MenuCategoryId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MenuCategoryId",
                table: "Menus");

            migrationBuilder.AddColumn<long>(
                name: "ItemId",
                table: "OrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Items",
                table: "OrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MenuCategoryId",
                table: "Items",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_Items",
                table: "OrderDetails",
                column: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_Items_MenuCategoryId",
                table: "Items",
                column: "MenuCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_MenuCategories_MenuCategoryId",
                table: "Items",
                column: "MenuCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Items_Items",
                table: "OrderDetails",
                column: "Items",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_MenuCategories_MenuCategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Items_Items",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_Items",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Items_MenuCategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Items",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MenuCategoryId",
                table: "Items");

            migrationBuilder.AddColumn<long>(
                name: "MenuId",
                table: "OrderDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MenuCategoryId",
                table: "Menus",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MenuId",
                table: "OrderDetails",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_MenuCategoryId",
                table: "Menus",
                column: "MenuCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_MenuCategories_MenuCategoryId",
                table: "Menus",
                column: "MenuCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Menus_MenuId",
                table: "OrderDetails",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
