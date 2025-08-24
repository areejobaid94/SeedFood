using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddChangeToItems3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_MenuCategories_MenuCategoryId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "MenuCategories");

            migrationBuilder.DropIndex(
                name: "IX_Items_MenuCategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MenuCategoryId",
                table: "Items");

            migrationBuilder.AddColumn<long>(
                name: "ItemCategoryId",
                table: "Items",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemCategorys",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCategorys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemCategoryId",
                table: "Items",
                column: "ItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCategorys_TenantId",
                table: "ItemCategorys",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemCategorys_ItemCategoryId",
                table: "Items",
                column: "ItemCategoryId",
                principalTable: "ItemCategorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemCategorys_ItemCategoryId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "ItemCategorys");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemCategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemCategoryId",
                table: "Items");

            migrationBuilder.AddColumn<long>(
                name: "MenuCategoryId",
                table: "Items",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MenuCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_MenuCategoryId",
                table: "Items",
                column: "MenuCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_TenantId",
                table: "MenuCategories",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_MenuCategories_MenuCategoryId",
                table: "Items",
                column: "MenuCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
