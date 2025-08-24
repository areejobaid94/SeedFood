using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addChangesDeliveryLocationCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "RestaurantMenuType",
            //    table: "Branches",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DeliveryLocationCost",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    FromLocationId = table.Column<int>(nullable: false),
                    ToLocationId = table.Column<int>(nullable: false),
                    DeliveryCost = table.Column<decimal>(nullable: false),
                    BranchAreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryLocationCost", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryLocationCost");

            //migrationBuilder.DropColumn(
            //    name: "RestaurantMenuType",
            //    table: "Branches");
        }
    }
}
