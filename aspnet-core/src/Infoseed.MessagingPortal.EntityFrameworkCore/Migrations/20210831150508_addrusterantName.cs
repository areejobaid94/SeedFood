using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addrusterantName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Billings_TenantServices_TenantServiceId",
            //    table: "Billings");

            //migrationBuilder.DropIndex(
            //    name: "IX_Billings_TenantServiceId",
            //    table: "Billings");

            //migrationBuilder.DropColumn(
            //    name: "TenantServiceId",
            //    table: "Billings");

            //migrationBuilder.AddColumn<decimal>(
            //    name: "FeesForFirstOrder",
            //    table: "TenantServices",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<int>(
            //    name: "FirstNumberOfOrders",
            //    table: "TenantServices",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RestaurantName",
                table: "Orders",
                nullable: true);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "FeesForFirstOrder",
            //    table: "InfoSeedServices",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<int>(
            //    name: "FirstNumberOfOrders",
            //    table: "InfoSeedServices",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "Qty",
            //    table: "AccountBillings",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "CostPerOrder",
            //    table: "AbpTenants",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<int>(
            //    name: "DueDay",
            //    table: "AbpTenants",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "SubscriptionFees",
            //    table: "AbpTenants",
            //    nullable: false,
            //    defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "FeesForFirstOrder",
            //    table: "TenantServices");

            //migrationBuilder.DropColumn(
            //    name: "FirstNumberOfOrders",
            //    table: "TenantServices");

            migrationBuilder.DropColumn(
                name: "RestaurantName",
                table: "Orders");

            //migrationBuilder.DropColumn(
            //    name: "FeesForFirstOrder",
            //    table: "InfoSeedServices");

            //migrationBuilder.DropColumn(
            //    name: "FirstNumberOfOrders",
            //    table: "InfoSeedServices");

            //migrationBuilder.DropColumn(
            //    name: "Qty",
            //    table: "AccountBillings");

            //migrationBuilder.DropColumn(
            //    name: "CostPerOrder",
            //    table: "AbpTenants");

            //migrationBuilder.DropColumn(
            //    name: "DueDay",
            //    table: "AbpTenants");

            //migrationBuilder.DropColumn(
            //    name: "SubscriptionFees",
            //    table: "AbpTenants");

            //migrationBuilder.AddColumn<int>(
            //    name: "TenantServiceId",
            //    table: "Billings",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Billings_TenantServiceId",
            //    table: "Billings",
            //    column: "TenantServiceId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Billings_TenantServices_TenantServiceId",
            //    table: "Billings",
            //    column: "TenantServiceId",
            //    principalTable: "TenantServices",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
