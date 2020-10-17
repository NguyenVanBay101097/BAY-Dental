using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateTCareTablesOptimizeV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "TCareMessagings");

            migrationBuilder.AddColumn<string>(
                name: "MessagingModel",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDate",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TCareMessagingPartnerRels",
                columns: table => new
                {
                    MessagingId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareMessagingPartnerRels", x => new { x.MessagingId, x.PartnerId });
                    table.ForeignKey(
                        name: "FK_TCareMessagingPartnerRels_TCareMessagings_MessagingId",
                        column: x => x.MessagingId,
                        principalTable: "TCareMessagings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TCareMessagingPartnerRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagingPartnerRels_PartnerId",
                table: "TCareMessagingPartnerRels",
                column: "PartnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCareMessagingPartnerRels");

            migrationBuilder.DropColumn(
                name: "MessagingModel",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "TCareMessagings");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "TCareMessagings",
                type: "datetime2",
                nullable: true);
        }
    }
}
