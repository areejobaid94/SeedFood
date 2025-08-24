using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class addSpecifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "ItemSpecifications",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(nullable: true),
            //        ItemId = table.Column<int>(nullable: false),
            //        SpecificationId = table.Column<int>(nullable: false),
            //        IsRequired = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ItemSpecifications", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SpecificationChoices",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(nullable: true),
            //        SpecificationChoiceDescription = table.Column<string>(nullable: true),
            //        SpecificationChoiceDescriptionEnglish = table.Column<string>(nullable: true),
            //        SKU = table.Column<string>(nullable: true),
            //        LanguageBotId = table.Column<int>(nullable: false),
            //        SpecificationId = table.Column<int>(nullable: false),
            //        Price = table.Column<decimal>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SpecificationChoices", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Specifications",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TenantId = table.Column<int>(nullable: true),
            //        SpecificationDescription = table.Column<string>(nullable: true),
            //        SpecificationDescriptionEnglish = table.Column<string>(nullable: true),
            //        IsMultipleSelection = table.Column<bool>(nullable: false),
            //        LanguageBotId = table.Column<int>(nullable: false),
            //        LastModificationTime = table.Column<DateTime>(nullable: false),
            //        CreatorUserId = table.Column<int>(nullable: false),
            //        LastModifierUserId = table.Column<int>(nullable: false),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        DeleterUserId = table.Column<int>(nullable: false),
            //        CreationTime = table.Column<DateTime>(nullable: false),
            //        DeletionTime = table.Column<DateTime>(nullable: false),
            //        Priority = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Specifications", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "ItemSpecifications");

            //migrationBuilder.DropTable(
            //    name: "SpecificationChoices");

            //migrationBuilder.DropTable(
            //    name: "Specifications");
        }
    }
}
