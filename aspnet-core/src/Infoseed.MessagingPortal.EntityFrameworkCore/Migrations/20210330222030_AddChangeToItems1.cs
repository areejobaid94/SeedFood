using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddChangeToItems1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuDetails_Items_ItemId",
                table: "MenuDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuDetails_Menus_MenuId",
                table: "MenuDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuDetails_MenuItemStatuses_MenuItemStatusId",
                table: "MenuDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_MenuItemStatuses_MenuItemStatusId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_MenuItemStatusId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_MenuDetails_ItemId",
                table: "MenuDetails");

            migrationBuilder.DropIndex(
                name: "IX_MenuDetails_MenuId",
                table: "MenuDetails");

            migrationBuilder.DropIndex(
                name: "IX_MenuDetails_MenuItemStatusId",
                table: "MenuDetails");

            migrationBuilder.DropColumn(
                name: "MenuItemStatusId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "MenuDetails");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "MenuDetails");

            migrationBuilder.DropColumn(
                name: "MenuItemStatusId",
                table: "MenuDetails");

            migrationBuilder.AddColumn<long>(
                name: "MenuId",
                table: "Items",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_MenuId",
                table: "Items",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Menus_MenuId",
                table: "Items",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Menus_MenuId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_MenuId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "Items");

            migrationBuilder.AddColumn<long>(
                name: "MenuItemStatusId",
                table: "Menus",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ItemId",
                table: "MenuDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MenuId",
                table: "MenuDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MenuItemStatusId",
                table: "MenuDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_MenuItemStatusId",
                table: "Menus",
                column: "MenuItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuDetails_ItemId",
                table: "MenuDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuDetails_MenuId",
                table: "MenuDetails",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuDetails_MenuItemStatusId",
                table: "MenuDetails",
                column: "MenuItemStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuDetails_Items_ItemId",
                table: "MenuDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuDetails_Menus_MenuId",
                table: "MenuDetails",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuDetails_MenuItemStatuses_MenuItemStatusId",
                table: "MenuDetails",
                column: "MenuItemStatusId",
                principalTable: "MenuItemStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_MenuItemStatuses_MenuItemStatusId",
                table: "Menus",
                column: "MenuItemStatusId",
                principalTable: "MenuItemStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
