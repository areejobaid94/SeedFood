using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddItemAdditionsCategory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ItemAdditionsCategory",
                table: "ItemAdditions",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ItemAdditionsCategoryId",
                table: "ItemAdditions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemAdditions_ItemAdditionsCategory",
                table: "ItemAdditions",
                column: "ItemAdditionsCategory");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemAdditions_ItemAdditionsCategorys_ItemAdditionsCategory",
                table: "ItemAdditions",
                column: "ItemAdditionsCategory",
                principalTable: "ItemAdditionsCategorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemAdditions_ItemAdditionsCategorys_ItemAdditionsCategory",
                table: "ItemAdditions");

            migrationBuilder.DropIndex(
                name: "IX_ItemAdditions_ItemAdditionsCategory",
                table: "ItemAdditions");

            migrationBuilder.DropColumn(
                name: "ItemAdditionsCategory",
                table: "ItemAdditions");

            migrationBuilder.DropColumn(
                name: "ItemAdditionsCategoryId",
                table: "ItemAdditions");
        }
    }
}
