using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Regenerated_TemplateMessage6010 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageTest",
                table: "TemplateMessages");

            migrationBuilder.AddColumn<DateTime>(
                name: "MessageCreationDate",
                table: "TemplateMessages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MessageText",
                table: "TemplateMessages",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageCreationDate",
                table: "TemplateMessages");

            migrationBuilder.DropColumn(
                name: "MessageText",
                table: "TemplateMessages");

            migrationBuilder.AddColumn<string>(
                name: "MessageTest",
                table: "TemplateMessages",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }
    }
}
