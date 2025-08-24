using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddChangeToItems6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                table: "Orders",
                type: "bigint",
                nullable: true);
        }
    }
}
