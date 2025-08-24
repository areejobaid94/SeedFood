using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Regenerated_MenuCategory6529 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemDescription = table.Column<string>(nullable: true),
                    Ingredients = table.Column<string>(nullable: true),
                    ItemName = table.Column<string>(maxLength: 450, nullable: false),
                    IsInService = table.Column<bool>(nullable: false),
                    CategoryNames = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuCategories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    MenuName = table.Column<string>(maxLength: 450, nullable: false),
                    MenuDescription = table.Column<string>(nullable: true),
                    EffectiveTimeFrom = table.Column<DateTime>(nullable: true),
                    EffectiveTimeTo = table.Column<DateTime>(nullable: true),
                    Tax = table.Column<decimal>(nullable: true),
                    ImageUri = table.Column<string>(maxLength: 450, nullable: true),
                    MenuItemStatusId = table.Column<long>(nullable: true),
                    MenuCategoryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_MenuCategories_MenuCategoryId",
                        column: x => x.MenuCategoryId,
                        principalTable: "MenuCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Menus_MenuItemStatuses_MenuItemStatusId",
                        column: x => x.MenuItemStatusId,
                        principalTable: "MenuItemStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsStandAlone = table.Column<bool>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    ItemId = table.Column<long>(nullable: false),
                    MenuId = table.Column<long>(nullable: false),
                    MenuItemStatusId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuDetails_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuDetails_MenuItemStatuses_MenuItemStatusId",
                        column: x => x.MenuItemStatusId,
                        principalTable: "MenuItemStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_TenantId",
                table: "Items",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_TenantId",
                table: "MenuCategories",
                column: "TenantId");

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

            migrationBuilder.CreateIndex(
                name: "IX_MenuDetails_TenantId",
                table: "MenuDetails",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_MenuCategoryId",
                table: "Menus",
                column: "MenuCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_MenuItemStatusId",
                table: "Menus",
                column: "MenuItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_TenantId",
                table: "Menus",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuDetails");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "MenuCategories");

            migrationBuilder.DropTable(
                name: "MenuItemStatuses");
        }
    }
}
