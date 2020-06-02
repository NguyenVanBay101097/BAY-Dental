using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateTCareMessagingTrace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TCareMessingTraces",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    TCareCampaignId = table.Column<Guid>(nullable: false),
                    Sent = table.Column<DateTime>(nullable: true),
                    Exception = table.Column<DateTime>(nullable: true),
                    Read = table.Column<DateTime>(nullable: true),
                    Delivery = table.Column<DateTime>(nullable: true),
                    MessageId = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareMessingTraces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareMessingTraces_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessingTraces_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TCareMessingTraces_TCareCampaigns_TCareCampaignId",
                        column: x => x.TCareCampaignId,
                        principalTable: "TCareCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TCareMessingTraces_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessingTraces_CreatedById",
                table: "TCareMessingTraces",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessingTraces_PartnerId",
                table: "TCareMessingTraces",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessingTraces_TCareCampaignId",
                table: "TCareMessingTraces",
                column: "TCareCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessingTraces_WriteById",
                table: "TCareMessingTraces",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCareMessingTraces");
        }
    }
}
