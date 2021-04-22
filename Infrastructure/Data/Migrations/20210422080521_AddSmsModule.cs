using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddSmsModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmsAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    BrandName = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    ClientSecret = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
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
                name: "SmsSmss",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ErrorCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsSmss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsSmss_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsSmss_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsSmss_AspNetUsers_WriteById",
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
                    Body = table.Column<string>(nullable: true)
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
                    IsBirthdayAutomation = table.Column<bool>(nullable: false),
                    BirthdayTemplateId = table.Column<Guid>(nullable: true),
                    IsAppointmentAutomation = table.Column<bool>(nullable: false),
                    AppointmentTemplateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_SmsTemplates_AppointmentTemplateId",
                        column: x => x.AppointmentTemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsConfigs_SmsTemplates_BirthdayTemplateId",
                        column: x => x.BirthdayTemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_SmsConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_SmsConfigs_AppointmentTemplateId",
                table: "SmsConfigs",
                column: "AppointmentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_BirthdayTemplateId",
                table: "SmsConfigs",
                column: "BirthdayTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_CompanyId",
                table: "SmsConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_CreatedById",
                table: "SmsConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_WriteById",
                table: "SmsConfigs",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_CreatedById",
                table: "SmsSmss",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_PartnerId",
                table: "SmsSmss",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_WriteById",
                table: "SmsSmss",
                column: "WriteById");

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
                name: "SmsAccounts");

            migrationBuilder.DropTable(
                name: "SmsComposers");

            migrationBuilder.DropTable(
                name: "SmsConfigs");

            migrationBuilder.DropTable(
                name: "SmsSmss");

            migrationBuilder.DropTable(
                name: "SmsTemplates");
        }
    }
}
