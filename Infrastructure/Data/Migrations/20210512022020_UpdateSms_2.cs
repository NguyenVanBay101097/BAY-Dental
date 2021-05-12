using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsSmss_SmsCampaign_SmsCampaignId",
                table: "SmsSmss");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsSmss_SmsConfigs_SmsConfigId",
                table: "SmsSmss");

            migrationBuilder.DropIndex(
                name: "IX_SmsSmss_SmsCampaignId",
                table: "SmsSmss");

            migrationBuilder.DropIndex(
                name: "IX_SmsSmss_SmsConfigId",
                table: "SmsSmss");

            migrationBuilder.DropColumn(
                name: "SmsCampaignId",
                table: "SmsSmss");

            migrationBuilder.DropColumn(
                name: "SmsConfigId",
                table: "SmsSmss");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "SmsConfigs");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "SmsSmss",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "SmsAccountId",
                table: "SmsSmss",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SmsMessageId",
                table: "SmsSmss",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmsMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SmsCampaignId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    TypeSend = table.Column<string>(nullable: true),
                    SmsTemplateId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsMessages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsMessages_SmsCampaign_SmsCampaignId",
                        column: x => x.SmsCampaignId,
                        principalTable: "SmsCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsMessages_SmsTemplates_SmsTemplateId",
                        column: x => x.SmsTemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsMessages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmsMessagePartnerRels",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(nullable: false),
                    SmsMessageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessagePartnerRels", x => new { x.PartnerId, x.SmsMessageId });
                    table.ForeignKey(
                        name: "FK_SmsMessagePartnerRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsMessagePartnerRels_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_SmsAccountId",
                table: "SmsSmss",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_SmsMessageId",
                table: "SmsSmss",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessagePartnerRels_SmsMessageId",
                table: "SmsMessagePartnerRels",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_CreatedById",
                table: "SmsMessages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_SmsCampaignId",
                table: "SmsMessages",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_SmsTemplateId",
                table: "SmsMessages",
                column: "SmsTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_WriteById",
                table: "SmsMessages",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsSmss_SmsAccounts_SmsAccountId",
                table: "SmsSmss",
                column: "SmsAccountId",
                principalTable: "SmsAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsSmss_SmsMessages_SmsMessageId",
                table: "SmsSmss",
                column: "SmsMessageId",
                principalTable: "SmsMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsSmss_SmsAccounts_SmsAccountId",
                table: "SmsSmss");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsSmss_SmsMessages_SmsMessageId",
                table: "SmsSmss");

            migrationBuilder.DropTable(
                name: "SmsMessagePartnerRels");

            migrationBuilder.DropTable(
                name: "SmsMessages");

            migrationBuilder.DropIndex(
                name: "IX_SmsSmss_SmsAccountId",
                table: "SmsSmss");

            migrationBuilder.DropIndex(
                name: "IX_SmsSmss_SmsMessageId",
                table: "SmsSmss");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "SmsSmss");

            migrationBuilder.DropColumn(
                name: "SmsAccountId",
                table: "SmsSmss");

            migrationBuilder.DropColumn(
                name: "SmsMessageId",
                table: "SmsSmss");

            migrationBuilder.AddColumn<Guid>(
                name: "SmsCampaignId",
                table: "SmsSmss",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SmsConfigId",
                table: "SmsSmss",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "SmsConfigs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_SmsCampaignId",
                table: "SmsSmss",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_SmsConfigId",
                table: "SmsSmss",
                column: "SmsConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsSmss_SmsCampaign_SmsCampaignId",
                table: "SmsSmss",
                column: "SmsCampaignId",
                principalTable: "SmsCampaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsSmss_SmsConfigs_SmsConfigId",
                table: "SmsSmss",
                column: "SmsConfigId",
                principalTable: "SmsConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
