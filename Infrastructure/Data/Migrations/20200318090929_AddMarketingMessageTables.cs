using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddMarketingMessageTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "MarketingCampaignActivities",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MarketingMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    Template = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingMessages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketingMessages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketingMessageButtons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Payload = table.Column<string>(nullable: true),
                    MessageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingMessageButtons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingMessageButtons_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketingMessageButtons_MarketingMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "MarketingMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketingMessageButtons_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignActivities_MessageId",
                table: "MarketingCampaignActivities",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingMessageButtons_CreatedById",
                table: "MarketingMessageButtons",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingMessageButtons_MessageId",
                table: "MarketingMessageButtons",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingMessageButtons_WriteById",
                table: "MarketingMessageButtons",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingMessages_CreatedById",
                table: "MarketingMessages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingMessages_WriteById",
                table: "MarketingMessages",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketingCampaignActivities_MarketingMessages_MessageId",
                table: "MarketingCampaignActivities",
                column: "MessageId",
                principalTable: "MarketingMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketingCampaignActivities_MarketingMessages_MessageId",
                table: "MarketingCampaignActivities");

            migrationBuilder.DropTable(
                name: "MarketingMessageButtons");

            migrationBuilder.DropTable(
                name: "MarketingMessages");

            migrationBuilder.DropIndex(
                name: "IX_MarketingCampaignActivities_MessageId",
                table: "MarketingCampaignActivities");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "MarketingCampaignActivities");
        }
    }
}
