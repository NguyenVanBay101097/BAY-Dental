using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddFixMigrationTCare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessagings_FacebookPages_ChannelSocialId",
                table: "TCareMessagings");

            migrationBuilder.DropTable(
                name: "TCareMessingTraces");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessagings_ChannelSocialId",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "ChannelSocialId",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "ChannelType",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "IntervalNumber",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "IntervalType",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "MethodType",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "SheduleDate",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "SheduleStart",
                table: "TCareCampaigns");

            migrationBuilder.AddColumn<string>(
                name: "AutoCustomType",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChannalType",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChannelSocialId",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomDay",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomHour",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomMinute",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomMonth",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CouponProgramId",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FacebookPageId",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessagingModel",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDate",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FacebookPageId",
                table: "TCareCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SheduleStartNumber",
                table: "TCareCampaigns",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SheduleStartType",
                table: "TCareCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "FacebookUserProfiles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TCareConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    JobCampaignHour = table.Column<int>(nullable: true),
                    JobCampaignMinute = table.Column<int>(nullable: true),
                    JobMessagingMinute = table.Column<int>(nullable: true),
                    JobMessageMinute = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    TCareMessagingId = table.Column<Guid>(nullable: true),
                    MessageContent = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ScheduledDate = table.Column<DateTime>(nullable: true),
                    Sent = table.Column<DateTime>(nullable: true),
                    Opened = table.Column<DateTime>(nullable: true),
                    Delivery = table.Column<DateTime>(nullable: true),
                    FailureReason = table.Column<string>(nullable: true),
                    MessageId = table.Column<string>(nullable: true)
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
                        name: "FK_TCareMessages_TCareMessagings_TCareMessagingId",
                        column: x => x.TCareMessagingId,
                        principalTable: "TCareMessagings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TCareMessageTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    CouponProgramId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareMessageTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareMessageTemplates_SaleCouponPrograms_CouponProgramId",
                        column: x => x.CouponProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessageTemplates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareMessageTemplates_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_TCareScenarios_ChannelSocialId",
                table: "TCareScenarios",
                column: "ChannelSocialId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagings_CouponProgramId",
                table: "TCareMessagings",
                column: "CouponProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagings_FacebookPageId",
                table: "TCareMessagings",
                column: "FacebookPageId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareCampaigns_FacebookPageId",
                table: "TCareCampaigns",
                column: "FacebookPageId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareConfigs_CreatedById",
                table: "TCareConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareConfigs_WriteById",
                table: "TCareConfigs",
                column: "WriteById");

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
                name: "IX_TCareMessages_TCareMessagingId",
                table: "TCareMessages",
                column: "TCareMessagingId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_WriteById",
                table: "TCareMessages",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessageTemplates_CouponProgramId",
                table: "TCareMessageTemplates",
                column: "CouponProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessageTemplates_CreatedById",
                table: "TCareMessageTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessageTemplates_WriteById",
                table: "TCareMessageTemplates",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagingPartnerRels_PartnerId",
                table: "TCareMessagingPartnerRels",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareCampaigns_FacebookPages_FacebookPageId",
                table: "TCareCampaigns",
                column: "FacebookPageId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessagings_SaleCouponPrograms_CouponProgramId",
                table: "TCareMessagings",
                column: "CouponProgramId",
                principalTable: "SaleCouponPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessagings_FacebookPages_FacebookPageId",
                table: "TCareMessagings",
                column: "FacebookPageId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCareScenarios_FacebookPages_ChannelSocialId",
                table: "TCareScenarios",
                column: "ChannelSocialId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareCampaigns_FacebookPages_FacebookPageId",
                table: "TCareCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessagings_SaleCouponPrograms_CouponProgramId",
                table: "TCareMessagings");

            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessagings_FacebookPages_FacebookPageId",
                table: "TCareMessagings");

            migrationBuilder.DropForeignKey(
                name: "FK_TCareScenarios_FacebookPages_ChannelSocialId",
                table: "TCareScenarios");

            migrationBuilder.DropTable(
                name: "TCareConfigs");

            migrationBuilder.DropTable(
                name: "TCareMessages");

            migrationBuilder.DropTable(
                name: "TCareMessageTemplates");

            migrationBuilder.DropTable(
                name: "TCareMessagingPartnerRels");

            migrationBuilder.DropIndex(
                name: "IX_TCareScenarios_ChannelSocialId",
                table: "TCareScenarios");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessagings_CouponProgramId",
                table: "TCareMessagings");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessagings_FacebookPageId",
                table: "TCareMessagings");

            migrationBuilder.DropIndex(
                name: "IX_TCareCampaigns_FacebookPageId",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "AutoCustomType",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "ChannalType",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "ChannelSocialId",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CustomDay",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CustomHour",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CustomMinute",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CustomMonth",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "CouponProgramId",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "FacebookPageId",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "MessagingModel",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "State",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "FacebookPageId",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "SheduleStartNumber",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "SheduleStartType",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "FacebookUserProfiles");

            migrationBuilder.AddColumn<Guid>(
                name: "ChannelSocialId",
                table: "TCareMessagings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChannelType",
                table: "TCareMessagings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntervalNumber",
                table: "TCareMessagings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntervalType",
                table: "TCareMessagings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MethodType",
                table: "TCareMessagings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SheduleDate",
                table: "TCareMessagings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SheduleStart",
                table: "TCareCampaigns",
                type: "datetime2",
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
                        name: "FK_TCareMessingTraces_AspNetUsers_WriteById",
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
                name: "IX_TCareMessingTraces_WriteById",
                table: "TCareMessingTraces",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessagings_FacebookPages_ChannelSocialId",
                table: "TCareMessagings",
                column: "ChannelSocialId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
