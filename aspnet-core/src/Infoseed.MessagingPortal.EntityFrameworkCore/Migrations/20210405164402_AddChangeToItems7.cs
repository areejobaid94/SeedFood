using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddChangeToItems7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryChanges_DeliveryChangeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryChangeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryChangeId",
                table: "Orders");

            migrationBuilder.AddColumn<long>(
                name: "AgentId",
                table: "Orders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsLockByAgent",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LockByAgentName",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsLockByAgent",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LockByAgentName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Orders");

            migrationBuilder.AddColumn<long>(
                name: "DeliveryChangeId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryChangeId",
                table: "Orders",
                column: "DeliveryChangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryChanges_DeliveryChangeId",
                table: "Orders",
                column: "DeliveryChangeId",
                principalTable: "DeliveryChanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
