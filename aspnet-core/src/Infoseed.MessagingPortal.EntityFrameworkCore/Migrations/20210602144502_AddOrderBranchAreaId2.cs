using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddOrderBranchAreaId2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "BranchAreaId",
            //    table: "Orders");

            //migrationBuilder.DropColumn(
            //    name: "BranchAreaName",
            //    table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "IsBranchArea",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBranchArea",
                table: "Orders");

            //migrationBuilder.AddColumn<int>(
            //    name: "BranchAreaId",
            //    table: "Orders",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<string>(
            //    name: "BranchAreaName",
            //    table: "Orders",
            //    type: "nvarchar(max)",
            //    nullable: true);
        }
    }
}
