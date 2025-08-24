using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addLangToMenuanditemandcatgor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Menus_LanguageBot_LanguageBotId",
            //    table: "Menus");

            //migrationBuilder.DropIndex(
            //    name: "IX_Menus_LanguageBotId",
            //    table: "Menus");

            migrationBuilder.AddColumn<int>(
                name: "LanguageBotId",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LanguageBotId",
                table: "ItemCategorys",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LanguageBotId",
                table: "ItemAdditions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageBotId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "LanguageBotId",
                table: "ItemCategorys");

            migrationBuilder.DropColumn(
                name: "LanguageBotId",
                table: "ItemAdditions");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Menus_LanguageBotId",
            //    table: "Menus",
            //    column: "LanguageBotId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Menus_LanguageBot_LanguageBotId",
            //    table: "Menus",
            //    column: "LanguageBotId",
            //    principalTable: "LanguageBot",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
