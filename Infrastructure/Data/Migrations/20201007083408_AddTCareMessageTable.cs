using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTCareMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannalType",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TCareMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ProfilePartnerId = table.Column<Guid>(nullable: true),
                    ChannelSocicalId = table.Column<Guid>(nullable: true),
                    CampaignId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    MessageContent = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareMessages_TCareCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "TCareCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessages_FacebookPages_ChannelSocicalId",
                        column: x => x.ChannelSocicalId,
                        principalTable: "FacebookPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessages_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessages_FacebookUserProfiles_ProfilePartnerId",
                        column: x => x.ProfilePartnerId,
                        principalTable: "FacebookUserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_CampaignId",
                table: "TCareMessages",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_ChannelSocicalId",
                table: "TCareMessages",
                column: "ChannelSocicalId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_CreatedById",
                table: "TCareMessages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_PartnerId",
                table: "TCareMessages",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_ProfilePartnerId",
                table: "TCareMessages",
                column: "ProfilePartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_WriteById",
                table: "TCareMessages",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "ChannalType",
                table: "TCareScenarios");
        }
    }
}
