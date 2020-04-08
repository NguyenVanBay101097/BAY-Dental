using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class MarketingActivityServerAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "MarketingTraces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "MarketingCampaignActivities",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MarketingCampaignActivityFacebookTagRels",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingCampaignActivityFacebookTagRels", x => new { x.ActivityId, x.TagId });
                    table.ForeignKey(
                        name: "FK_MarketingCampaignActivityFacebookTagRels_MarketingCampaignActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "MarketingCampaignActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketingCampaignActivityFacebookTagRels_FacebookTags_TagId",
                        column: x => x.TagId,
                        principalTable: "FacebookTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingTraces_UserProfileId",
                table: "MarketingTraces",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignActivityFacebookTagRels_TagId",
                table: "MarketingCampaignActivityFacebookTagRels",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketingTraces_FacebookUserProfiles_UserProfileId",
                table: "MarketingTraces",
                column: "UserProfileId",
                principalTable: "FacebookUserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketingTraces_FacebookUserProfiles_UserProfileId",
                table: "MarketingTraces");

            migrationBuilder.DropTable(
                name: "MarketingCampaignActivityFacebookTagRels");

            migrationBuilder.DropIndex(
                name: "IX_MarketingTraces_UserProfileId",
                table: "MarketingTraces");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "MarketingTraces");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "MarketingCampaignActivities");
        }
    }
}
