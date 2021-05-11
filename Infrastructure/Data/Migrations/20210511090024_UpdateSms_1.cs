using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_SmsTemplates_AppointmentTemplateId",
                table: "SmsConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_SmsTemplates_BirthdayTemplateId",
                table: "SmsConfigs");

            migrationBuilder.DropIndex(
                name: "IX_SmsConfigs_AppointmentTemplateId",
                table: "SmsConfigs");

            migrationBuilder.DropIndex(
                name: "IX_SmsConfigs_BirthdayTemplateId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "AppointmentTemplateId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "BirthdayTemplateId",
                table: "SmsConfigs");

            migrationBuilder.AddColumn<Guid>(
                name: "SmsCampaignId",
                table: "SmsSmss",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SmsConfigId",
                table: "SmsSmss",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "SmsConfigs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmsCampaign",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsCampaign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsCampaign_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsCampaign_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsCampaign_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_SmsCampaignId",
                table: "SmsSmss",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_SmsConfigId",
                table: "SmsSmss",
                column: "SmsConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_TemplateId",
                table: "SmsConfigs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCampaign_CompanyId",
                table: "SmsCampaign",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCampaign_CreatedById",
                table: "SmsCampaign",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCampaign_WriteById",
                table: "SmsCampaign",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_SmsTemplates_TemplateId",
                table: "SmsConfigs",
                column: "TemplateId",
                principalTable: "SmsTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_SmsTemplates_TemplateId",
                table: "SmsConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsSmss_SmsCampaign_SmsCampaignId",
                table: "SmsSmss");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsSmss_SmsConfigs_SmsConfigId",
                table: "SmsSmss");

            migrationBuilder.DropTable(
                name: "SmsCampaign");

            migrationBuilder.DropIndex(
                name: "IX_SmsSmss_SmsCampaignId",
                table: "SmsSmss");

            migrationBuilder.DropIndex(
                name: "IX_SmsSmss_SmsConfigId",
                table: "SmsSmss");

            migrationBuilder.DropIndex(
                name: "IX_SmsConfigs_TemplateId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "SmsCampaignId",
                table: "SmsSmss");

            migrationBuilder.DropColumn(
                name: "SmsConfigId",
                table: "SmsSmss");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "SmsConfigs");

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentTemplateId",
                table: "SmsConfigs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BirthdayTemplateId",
                table: "SmsConfigs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_AppointmentTemplateId",
                table: "SmsConfigs",
                column: "AppointmentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_BirthdayTemplateId",
                table: "SmsConfigs",
                column: "BirthdayTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_SmsTemplates_AppointmentTemplateId",
                table: "SmsConfigs",
                column: "AppointmentTemplateId",
                principalTable: "SmsTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_SmsTemplates_BirthdayTemplateId",
                table: "SmsConfigs",
                column: "BirthdayTemplateId",
                principalTable: "SmsTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
