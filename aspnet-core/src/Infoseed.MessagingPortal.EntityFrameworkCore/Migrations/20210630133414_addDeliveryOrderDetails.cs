using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addDeliveryOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    FromLocationId = table.Column<int>(nullable: false),
                    FromAddress = table.Column<string>(nullable: true),
                    FromGoogleURL = table.Column<string>(nullable: true),
                    ToLocationId = table.Column<int>(nullable: false),
                    ToAddress = table.Column<string>(nullable: true),
                    ToGoogleURL = table.Column<string>(nullable: true),
                    DeliveryCost = table.Column<decimal>(nullable: false),
                    DeliveryCostString = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOrderDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryOrderDetails");
        }
    }
}
