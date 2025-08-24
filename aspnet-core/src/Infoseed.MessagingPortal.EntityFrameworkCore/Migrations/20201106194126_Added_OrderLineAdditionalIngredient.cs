using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_OrderLineAdditionalIngredient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderLineAdditionalIngredients",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Total = table.Column<decimal>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    UnitPrice = table.Column<decimal>(nullable: true),
                    OrderId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLineAdditionalIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLineAdditionalIngredients_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineAdditionalIngredients_OrderId",
                table: "OrderLineAdditionalIngredients",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineAdditionalIngredients_TenantId",
                table: "OrderLineAdditionalIngredients",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderLineAdditionalIngredients");
        }
    }
}
