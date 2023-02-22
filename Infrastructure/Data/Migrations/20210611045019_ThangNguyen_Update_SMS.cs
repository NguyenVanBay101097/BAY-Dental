using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class ThangNguyen_Update_SMS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigProductCategoryRels_SmsConfigs_SmsConfigId",
                table: "SmsConfigProductCategoryRels");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigProductRels_SmsConfigs_SmsConfigId",
                table: "SmsConfigProductRels");

            migrationBuilder.DropTable(
                name: "SmsConfigs");

            migrationBuilder.CreateTable(
                name: "SmsAppointmentAutomationConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    SmsAccountId = table.Column<Guid>(nullable: true),
                    SmsCampaignId = table.Column<Guid>(nullable: true),
                    TimeBeforSend = table.Column<int>(nullable: false),
                    TypeTimeBeforSend = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    TemplateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsAppointmentAutomationConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsAppointmentAutomationConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsAppointmentAutomationConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsAppointmentAutomationConfigs_SmsAccounts_SmsAccountId",
                        column: x => x.SmsAccountId,
                        principalTable: "SmsAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsAppointmentAutomationConfigs_SmsCampaign_SmsCampaignId",
                        column: x => x.SmsCampaignId,
                        principalTable: "SmsCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsAppointmentAutomationConfigs_SmsTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsAppointmentAutomationConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmsBirthdayAutomationConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    SmsAccountId = table.Column<Guid>(nullable: true),
                    SmsCampaignId = table.Column<Guid>(nullable: true),
                    ScheduleTime = table.Column<DateTime>(nullable: true),
                    DayBeforeSend = table.Column<int>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    TemplateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsBirthdayAutomationConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsBirthdayAutomationConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsBirthdayAutomationConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsBirthdayAutomationConfigs_SmsAccounts_SmsAccountId",
                        column: x => x.SmsAccountId,
                        principalTable: "SmsAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsBirthdayAutomationConfigs_SmsCampaign_SmsCampaignId",
                        column: x => x.SmsCampaignId,
                        principalTable: "SmsCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsBirthdayAutomationConfigs_SmsTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsBirthdayAutomationConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmsCareAfterOrderAutomationConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    SmsAccountId = table.Column<Guid>(nullable: true),
                    SmsCampaignId = table.Column<Guid>(nullable: true),
                    ScheduleTime = table.Column<DateTime>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    TimeBeforSend = table.Column<int>(nullable: false),
                    TypeTimeBeforSend = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    TemplateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsCareAfterOrderAutomationConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsCareAfterOrderAutomationConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsCareAfterOrderAutomationConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsCareAfterOrderAutomationConfigs_SmsAccounts_SmsAccountId",
                        column: x => x.SmsAccountId,
                        principalTable: "SmsAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsCareAfterOrderAutomationConfigs_SmsCampaign_SmsCampaignId",
                        column: x => x.SmsCampaignId,
                        principalTable: "SmsCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsCareAfterOrderAutomationConfigs_SmsTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsCareAfterOrderAutomationConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmsThanksCustomerAutomationConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    SmsAccountId = table.Column<Guid>(nullable: true),
                    SmsCampaignId = table.Column<Guid>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    TimeBeforSend = table.Column<int>(nullable: false),
                    TypeTimeBeforSend = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    TemplateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsThanksCustomerAutomationConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsThanksCustomerAutomationConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsThanksCustomerAutomationConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsThanksCustomerAutomationConfigs_SmsAccounts_SmsAccountId",
                        column: x => x.SmsAccountId,
                        principalTable: "SmsAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsThanksCustomerAutomationConfigs_SmsCampaign_SmsCampaignId",
                        column: x => x.SmsCampaignId,
                        principalTable: "SmsCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsThanksCustomerAutomationConfigs_SmsTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsThanksCustomerAutomationConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsAppointmentAutomationConfigs_CompanyId",
                table: "SmsAppointmentAutomationConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsAppointmentAutomationConfigs_CreatedById",
                table: "SmsAppointmentAutomationConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsAppointmentAutomationConfigs_SmsAccountId",
                table: "SmsAppointmentAutomationConfigs",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsAppointmentAutomationConfigs_SmsCampaignId",
                table: "SmsAppointmentAutomationConfigs",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsAppointmentAutomationConfigs_TemplateId",
                table: "SmsAppointmentAutomationConfigs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsAppointmentAutomationConfigs_WriteById",
                table: "SmsAppointmentAutomationConfigs",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsBirthdayAutomationConfigs_CompanyId",
                table: "SmsBirthdayAutomationConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsBirthdayAutomationConfigs_CreatedById",
                table: "SmsBirthdayAutomationConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsBirthdayAutomationConfigs_SmsAccountId",
                table: "SmsBirthdayAutomationConfigs",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsBirthdayAutomationConfigs_SmsCampaignId",
                table: "SmsBirthdayAutomationConfigs",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsBirthdayAutomationConfigs_TemplateId",
                table: "SmsBirthdayAutomationConfigs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsBirthdayAutomationConfigs_WriteById",
                table: "SmsBirthdayAutomationConfigs",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCareAfterOrderAutomationConfigs_CompanyId",
                table: "SmsCareAfterOrderAutomationConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCareAfterOrderAutomationConfigs_CreatedById",
                table: "SmsCareAfterOrderAutomationConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCareAfterOrderAutomationConfigs_SmsAccountId",
                table: "SmsCareAfterOrderAutomationConfigs",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCareAfterOrderAutomationConfigs_SmsCampaignId",
                table: "SmsCareAfterOrderAutomationConfigs",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCareAfterOrderAutomationConfigs_TemplateId",
                table: "SmsCareAfterOrderAutomationConfigs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsCareAfterOrderAutomationConfigs_WriteById",
                table: "SmsCareAfterOrderAutomationConfigs",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsThanksCustomerAutomationConfigs_CompanyId",
                table: "SmsThanksCustomerAutomationConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsThanksCustomerAutomationConfigs_CreatedById",
                table: "SmsThanksCustomerAutomationConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsThanksCustomerAutomationConfigs_SmsAccountId",
                table: "SmsThanksCustomerAutomationConfigs",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsThanksCustomerAutomationConfigs_SmsCampaignId",
                table: "SmsThanksCustomerAutomationConfigs",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsThanksCustomerAutomationConfigs_TemplateId",
                table: "SmsThanksCustomerAutomationConfigs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsThanksCustomerAutomationConfigs_WriteById",
                table: "SmsThanksCustomerAutomationConfigs",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigProductCategoryRels_SmsCareAfterOrderAutomationConfigs_SmsConfigId",
                table: "SmsConfigProductCategoryRels",
                column: "SmsConfigId",
                principalTable: "SmsCareAfterOrderAutomationConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigProductRels_SmsCareAfterOrderAutomationConfigs_SmsConfigId",
                table: "SmsConfigProductRels",
                column: "SmsConfigId",
                principalTable: "SmsCareAfterOrderAutomationConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigProductCategoryRels_SmsCareAfterOrderAutomationConfigs_SmsConfigId",
                table: "SmsConfigProductCategoryRels");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigProductRels_SmsCareAfterOrderAutomationConfigs_SmsConfigId",
                table: "SmsConfigProductRels");

            migrationBuilder.DropTable(
                name: "SmsAppointmentAutomationConfigs");

            migrationBuilder.DropTable(
                name: "SmsBirthdayAutomationConfigs");

            migrationBuilder.DropTable(
                name: "SmsCareAfterOrderAutomationConfigs");

            migrationBuilder.DropTable(
                name: "SmsThanksCustomerAutomationConfigs");

            migrationBuilder.CreateTable(
                name: "SmsConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateSend = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAppointmentAutomation = table.Column<bool>(type: "bit", nullable: false),
                    IsBirthdayAutomation = table.Column<bool>(type: "bit", nullable: false),
                    IsCareAfterOrderAutomation = table.Column<bool>(type: "bit", nullable: false),
                    IsThanksCustomerAutomation = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SmsCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeBeforSend = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeTimeBeforSend = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WriteById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_SmsAccounts_SmsAccountId",
                        column: x => x.SmsAccountId,
                        principalTable: "SmsAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_SmsCampaign_SmsCampaignId",
                        column: x => x.SmsCampaignId,
                        principalTable: "SmsCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_SmsTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_CompanyId",
                table: "SmsConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_CreatedById",
                table: "SmsConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_SmsAccountId",
                table: "SmsConfigs",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_SmsCampaignId",
                table: "SmsConfigs",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_TemplateId",
                table: "SmsConfigs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_WriteById",
                table: "SmsConfigs",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigProductCategoryRels_SmsConfigs_SmsConfigId",
                table: "SmsConfigProductCategoryRels",
                column: "SmsConfigId",
                principalTable: "SmsConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigProductRels_SmsConfigs_SmsConfigId",
                table: "SmsConfigProductRels",
                column: "SmsConfigId",
                principalTable: "SmsConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
