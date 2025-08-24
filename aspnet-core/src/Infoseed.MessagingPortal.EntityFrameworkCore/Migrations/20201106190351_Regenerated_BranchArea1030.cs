using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Regenerated_BranchArea1030 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "BranchAreas",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_BranchAreas_BranchId",
                table: "BranchAreas",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchAreas_Branches_BranchId",
                table: "BranchAreas",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchAreas_Branches_BranchId",
                table: "BranchAreas");

            migrationBuilder.DropIndex(
                name: "IX_BranchAreas_BranchId",
                table: "BranchAreas");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "BranchAreas");
        }
    }
}
