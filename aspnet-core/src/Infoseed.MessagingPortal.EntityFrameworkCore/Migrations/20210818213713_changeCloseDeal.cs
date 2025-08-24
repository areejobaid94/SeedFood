using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class changeCloseDeal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CloseDate",
                table: "Close_Deals",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DealAge",
                table: "Close_Deals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealName",
                table: "Close_Deals",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DealStatusId",
                table: "Close_Deals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DealTypeId",
                table: "Close_Deals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Close_Deals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Close_Deals_DealStatusId",
                table: "Close_Deals",
                column: "DealStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Close_Deals_DealTypeId",
                table: "Close_Deals",
                column: "DealTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Close_Deals_DealStatuses_DealStatusId",
                table: "Close_Deals",
                column: "DealStatusId",
                principalTable: "DealStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Close_Deals_DealTypes_DealTypeId",
                table: "Close_Deals",
                column: "DealTypeId",
                principalTable: "DealTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Close_Deals_DealStatuses_DealStatusId",
                table: "Close_Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Close_Deals_DealTypes_DealTypeId",
                table: "Close_Deals");

            migrationBuilder.DropIndex(
                name: "IX_Close_Deals_DealStatusId",
                table: "Close_Deals");

            migrationBuilder.DropIndex(
                name: "IX_Close_Deals_DealTypeId",
                table: "Close_Deals");

            migrationBuilder.DropColumn(
                name: "CloseDate",
                table: "Close_Deals");

            migrationBuilder.DropColumn(
                name: "DealAge",
                table: "Close_Deals");

            migrationBuilder.DropColumn(
                name: "DealName",
                table: "Close_Deals");

            migrationBuilder.DropColumn(
                name: "DealStatusId",
                table: "Close_Deals");

            migrationBuilder.DropColumn(
                name: "DealTypeId",
                table: "Close_Deals");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Close_Deals");
        }
    }
}
