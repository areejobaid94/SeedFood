using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addLangToMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LanguageBotId",
                table: "Menus",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Menus_LanguageBot_LanguageBotId",
            //    table: "Menus");

            //migrationBuilder.DropIndex(
            //    name: "IX_Menus_LanguageBotId",
            //    table: "Menus");

            migrationBuilder.DropColumn(
                name: "LanguageBotId",
                table: "Menus");
        }
    }
}
