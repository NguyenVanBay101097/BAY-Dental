using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateTCareTablesOptimizeV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessages_TCareMessingTraces_TCareMessagingTraceId",
                table: "TCareMessages");

            migrationBuilder.DropTable(
                name: "TCareMessingTraces");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessages_TCareMessagingTraceId",
                table: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "CountPartner",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "TCareMessagingTraceId",
                table: "TCareMessages");

            migrationBuilder.AddColumn<DateTime>(
                name: "Delivery",
                table: "TCareMessages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "TCareMessages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "TCareMessages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Opened",
                table: "TCareMessages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Sent",
                table: "TCareMessages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Delivery",
                table: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "Opened",
                table: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "Sent",
                table: "TCareMessages");

            migrationBuilder.AddColumn<int>(
                name: "CountPartner",
                table: "TCareMessagings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TCareMessagingTraceId",
                table: "TCareMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TCareMessingTraces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelSocialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exception = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MessageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Opened = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PSID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Sent = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TCareCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TCareMessagingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WriteById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareMessingTraces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareMessingTraces_FacebookPages_ChannelSocialId",
                        column: x => x.ChannelSocialId,
                        principalTable: "FacebookPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        name: "FK_TCareMessingTraces_TCareMessagings_TCareMessagingId",
                        column: x => x.TCareMessagingId,
                        principalTable: "TCareMessagings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessingTraces_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_TCareMessagingTraceId",
                table: "TCareMessages",
                column: "TCareMessagingTraceId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessingTraces_ChannelSocialId",
                table: "TCareMessingTraces",
                column: "ChannelSocialId");

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
                name: "IX_TCareMessingTraces_TCareMessagingId",
                table: "TCareMessingTraces",
                column: "TCareMessagingId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessingTraces_WriteById",
                table: "TCareMessingTraces",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessages_TCareMessingTraces_TCareMessagingTraceId",
                table: "TCareMessages",
                column: "TCareMessagingTraceId",
                principalTable: "TCareMessingTraces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
