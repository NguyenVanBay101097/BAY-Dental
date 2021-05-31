using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddSMSModuleTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateAppointmentReminder",
                table: "Appointments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeAppointment",
                table: "Appointments",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmsAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    BrandName = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    ClientSecret = table.Column<string>(nullable: true),
                    ApiKey = table.Column<string>(nullable: true),
                    Secretkey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsAccounts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsAccounts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsAccounts_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    CompanyId = table.Column<Guid>(nullable: false),
                    LimitMessage = table.Column<int>(nullable: false),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    TypeDate = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    DefaultType = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "SmsTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsTemplates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsTemplates_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmsComposers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompositionMode = table.Column<string>(nullable: true),
                    ResModel = table.Column<string>(nullable: true),
                    ResIds = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    TemplateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsComposers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsComposers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsComposers_SmsTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsComposers_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmsConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    SmsAccountId = table.Column<Guid>(nullable: true),
                    SmsCampaignId = table.Column<Guid>(nullable: true),
                    DateSend = table.Column<DateTime>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    TimeBeforSend = table.Column<int>(nullable: false),
                    TypeTimeBeforSend = table.Column<string>(nullable: true),
                    IsBirthdayAutomation = table.Column<bool>(nullable: false),
                    IsAppointmentAutomation = table.Column<bool>(nullable: false),
                    IsCareAfterOrderAutomation = table.Column<bool>(nullable: false),
                    IsThanksCustomerAutomation = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    TemplateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Body = table.Column<string>(nullable: true),
                    SmsAccountId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ResModel = table.Column<string>(nullable: true)
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
                        name: "FK_SmsMessages_SmsAccounts_SmsAccountId",
                        column: x => x.SmsAccountId,
                        principalTable: "SmsAccounts",
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
                name: "SmsMessageAppointmentRels",
                columns: table => new
                {
                    SmsMessageId = table.Column<Guid>(nullable: false),
                    AppointmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessageAppointmentRels", x => new { x.AppointmentId, x.SmsMessageId });
                    table.ForeignKey(
                        name: "FK_SmsMessageAppointmentRels_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsMessageAppointmentRels_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmsMessageDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    Cost = table.Column<decimal>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true),
                    SmsAccountId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    ErrorCode = table.Column<string>(nullable: true),
                    SmsMessageId = table.Column<Guid>(nullable: true),
                    SmsCampaignId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessageDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsMessageDetails_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsMessageDetails_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsMessageDetails_SmsAccounts_SmsAccountId",
                        column: x => x.SmsAccountId,
                        principalTable: "SmsAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsMessageDetails_SmsCampaign_SmsCampaignId",
                        column: x => x.SmsCampaignId,
                        principalTable: "SmsCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsMessageDetails_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsMessageDetails_AspNetUsers_WriteById",
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

            migrationBuilder.CreateTable(
                name: "SmsMessageSaleOrderLineRels",
                columns: table => new
                {
                    SmsMessageId = table.Column<Guid>(nullable: false),
                    SaleOrderLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessageSaleOrderLineRels", x => new { x.SaleOrderLineId, x.SmsMessageId });
                    table.ForeignKey(
                        name: "FK_SmsMessageSaleOrderLineRels_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsMessageSaleOrderLineRels_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmsMessageSaleOrderRels",
                columns: table => new
                {
                    SmsMessageId = table.Column<Guid>(nullable: false),
                    SaleOrderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessageSaleOrderRels", x => new { x.SaleOrderId, x.SmsMessageId });
                    table.ForeignKey(
                        name: "FK_SmsMessageSaleOrderRels_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsMessageSaleOrderRels_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsAccounts_CompanyId",
                table: "SmsAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsAccounts_CreatedById",
                table: "SmsAccounts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsAccounts_WriteById",
                table: "SmsAccounts",
                column: "WriteById");

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

            migrationBuilder.CreateIndex(
                name: "IX_SmsComposers_CreatedById",
                table: "SmsComposers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsComposers_TemplateId",
                table: "SmsComposers",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsComposers_WriteById",
                table: "SmsComposers",
                column: "WriteById");

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

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageAppointmentRels_SmsMessageId",
                table: "SmsMessageAppointmentRels",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_CreatedById",
                table: "SmsMessageDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_PartnerId",
                table: "SmsMessageDetails",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_SmsAccountId",
                table: "SmsMessageDetails",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_SmsCampaignId",
                table: "SmsMessageDetails",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_SmsMessageId",
                table: "SmsMessageDetails",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_WriteById",
                table: "SmsMessageDetails",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessagePartnerRels_SmsMessageId",
                table: "SmsMessagePartnerRels",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_CreatedById",
                table: "SmsMessages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_SmsAccountId",
                table: "SmsMessages",
                column: "SmsAccountId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageSaleOrderLineRels_SmsMessageId",
                table: "SmsMessageSaleOrderLineRels",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageSaleOrderRels_SmsMessageId",
                table: "SmsMessageSaleOrderRels",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplates_CreatedById",
                table: "SmsTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplates_WriteById",
                table: "SmsTemplates",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmsComposers");

            migrationBuilder.DropTable(
                name: "SmsConfigs");

            migrationBuilder.DropTable(
                name: "SmsMessageAppointmentRels");

            migrationBuilder.DropTable(
                name: "SmsMessageDetails");

            migrationBuilder.DropTable(
                name: "SmsMessagePartnerRels");

            migrationBuilder.DropTable(
                name: "SmsMessageSaleOrderLineRels");

            migrationBuilder.DropTable(
                name: "SmsMessageSaleOrderRels");

            migrationBuilder.DropTable(
                name: "SmsMessages");

            migrationBuilder.DropTable(
                name: "SmsAccounts");

            migrationBuilder.DropTable(
                name: "SmsCampaign");

            migrationBuilder.DropTable(
                name: "SmsTemplates");

            migrationBuilder.DropColumn(
                name: "DateAppointmentReminder",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DateTimeAppointment",
                table: "Appointments");
        }
    }
}
