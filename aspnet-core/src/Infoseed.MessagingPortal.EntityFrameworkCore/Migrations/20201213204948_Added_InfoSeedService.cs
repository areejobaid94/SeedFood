using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infoseed.MessagingPortal.Migrations
{
    public partial class Added_InfoSeedService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfoSeedServices",
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
                    ServiceID = table.Column<string>(maxLength: 1024, nullable: false),
                    ServiceFees = table.Column<decimal>(nullable: false),
                    ServiceName = table.Column<string>(maxLength: 1024, nullable: false),
                    ServiceCreationDate = table.Column<DateTime>(nullable: false),
                    ServiceStoppingDate = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    ServiceTypeId = table.Column<int>(nullable: false),
                    ServiceStatusId = table.Column<int>(nullable: false),
                    ServiceFrquencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoSeedServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfoSeedServices_ServiceFrquencies_ServiceFrquencyId",
                        column: x => x.ServiceFrquencyId,
                        principalTable: "ServiceFrquencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InfoSeedServices_ServiceStatuses_ServiceStatusId",
                        column: x => x.ServiceStatusId,
                        principalTable: "ServiceStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InfoSeedServices_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfoSeedServices_ServiceFrquencyId",
                table: "InfoSeedServices",
                column: "ServiceFrquencyId");

            migrationBuilder.CreateIndex(
                name: "IX_InfoSeedServices_ServiceStatusId",
                table: "InfoSeedServices",
                column: "ServiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_InfoSeedServices_ServiceTypeId",
                table: "InfoSeedServices",
                column: "ServiceTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoSeedServices");
        }
    }
}
