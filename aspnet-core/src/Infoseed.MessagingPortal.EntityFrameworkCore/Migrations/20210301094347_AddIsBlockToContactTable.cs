using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class AddIsBlockToContactTable : Migration
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

            migrationBuilder.AddColumn<bool>(
                name: "IsBlock",
                table: "Contacts",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_TenantInformations",
            //    table: "TenantInformations",
            //    column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_TenantInformations",
            //    table: "TenantInformations");

            migrationBuilder.DropColumn(
                name: "IsBlock",
                table: "Contacts");

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
