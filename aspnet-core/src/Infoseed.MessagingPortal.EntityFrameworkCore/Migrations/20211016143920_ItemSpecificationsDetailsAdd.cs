using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class ItemSpecificationsDetailsAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemSpecificationsDetail",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CopiedFromId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: false),
                    MenuType = table.Column<int>(nullable: false),
                    SpecificationChoicesId = table.Column<int>(nullable: false),
                    IsInService = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSpecificationsDetail", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemSpecificationsDetail");
        }
    }
}
