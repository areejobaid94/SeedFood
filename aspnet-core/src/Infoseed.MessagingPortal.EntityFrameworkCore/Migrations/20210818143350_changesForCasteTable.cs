using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class changesForCasteTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ARR",
                table: "Forcasts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CloseDate",
                table: "Forcasts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Forcasts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealAge",
                table: "Forcasts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealName",
                table: "Forcasts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DealStatusId",
                table: "Forcasts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DealTypeId",
                table: "Forcasts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OrderFees",
                table: "Forcasts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Forcasts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forcasts_DealStatusId",
                table: "Forcasts",
                column: "DealStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Forcasts_DealTypeId",
                table: "Forcasts",
                column: "DealTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forcasts_DealStatuses_DealStatusId",
                table: "Forcasts",
                column: "DealStatusId",
                principalTable: "DealStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Forcasts_DealTypes_DealTypeId",
                table: "Forcasts",
                column: "DealTypeId",
                principalTable: "DealTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forcasts_DealStatuses_DealStatusId",
                table: "Forcasts");

            migrationBuilder.DropForeignKey(
                name: "FK_Forcasts_DealTypes_DealTypeId",
                table: "Forcasts");

            migrationBuilder.DropIndex(
                name: "IX_Forcasts_DealStatusId",
                table: "Forcasts");

            migrationBuilder.DropIndex(
                name: "IX_Forcasts_DealTypeId",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "ARR",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "CloseDate",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "DealAge",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "DealName",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "DealStatusId",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "DealTypeId",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "OrderFees",
                table: "Forcasts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Forcasts");
        }
    }
}
