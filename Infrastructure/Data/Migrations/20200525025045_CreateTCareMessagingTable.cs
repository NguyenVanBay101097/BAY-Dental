using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateTCareMessagingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TCareMessagings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    MethodType = table.Column<string>(nullable: true),
                    IntervalType = table.Column<string>(nullable: true),
                    IntervalNumber = table.Column<int>(nullable: true),
                    SheduleDate = table.Column<DateTime>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    ChannelType = table.Column<string>(nullable: true),
                    ChannelSocialId = table.Column<Guid>(nullable: true),
                    TCareCampaignId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareMessagings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareMessagings_FacebookPages_ChannelSocialId",
                        column: x => x.ChannelSocialId,
                        principalTable: "FacebookPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessagings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessagings_TCareCampaigns_TCareCampaignId",
                        column: x => x.TCareCampaignId,
                        principalTable: "TCareCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TCareMessagings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagings_ChannelSocialId",
                table: "TCareMessagings",
                column: "ChannelSocialId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagings_CreatedById",
                table: "TCareMessagings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagings_TCareCampaignId",
                table: "TCareMessagings",
                column: "TCareCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagings_WriteById",
                table: "TCareMessagings",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCareMessagings");
        }
    }
}
