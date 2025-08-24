using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddAreaIDToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AreaId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AreaId",
                table: "Orders",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Areas_AreaId",
                table: "Orders",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Areas_AreaId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AreaId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Orders");
        }
    }
}
