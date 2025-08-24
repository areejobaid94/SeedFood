using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_DealType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Forcatses",
                table: "Forcatses");

            migrationBuilder.RenameTable(
                name: "Forcatses",
                newName: "Forcasts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Forcasts",
                table: "Forcasts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DealStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DealTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    Deal_Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DealTypes_TenantId",
                table: "DealTypes",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DealStatuses");

            migrationBuilder.DropTable(
                name: "DealTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Forcasts",
                table: "Forcasts");

            migrationBuilder.RenameTable(
                name: "Forcasts",
                newName: "Forcatses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Forcatses",
                table: "Forcatses",
                column: "Id");
        }
    }
}
