using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class ContactNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_TenantInformations",
            //    table: "TenantInformations");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "TenantInformations",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .Annotation("SqlServer:Identity", "1, 1");

            //migrationBuilder.AlterColumn<int>(
            //    name: "TenantId",
            //    table: "TenantInformations",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_TenantInformations",
            //    table: "TenantInformations",
            //    column: "Id");

            migrationBuilder.CreateTable(
                name: "ContactNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ContactId = table.Column<string>(nullable: false),
                    NotificationId = table.Column<string>(nullable: false),
                    NotificationCreateDate = table.Column<DateTime>(nullable: false),
                    NotificationText = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactNotifications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactNotifications");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_TenantInformations",
            //    table: "TenantInformations");

            //migrationBuilder.AlterColumn<int>(
            //    name: "TenantId",
            //    table: "TenantInformations",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldNullable: true)
            //    .Annotation("SqlServer:Identity", "1, 1");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "TenantInformations",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_TenantInformations",
            //    table: "TenantInformations",
            //    column: "TenantId");
        }
    }
}
