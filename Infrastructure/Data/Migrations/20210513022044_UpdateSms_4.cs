using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_SmsCampaign_SmsCampaignId",
                table: "SmsConfigs");

            migrationBuilder.DropTable(
                name: "SmsSmss");

            migrationBuilder.DropIndex(
                name: "IX_SmsConfigs_SmsCampaignId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "SmsCampaignId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "SmsAccounts");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "SmsAccounts");

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
                    SmsMessageId = table.Column<Guid>(nullable: true)
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
                name: "IX_SmsMessageDetails_SmsMessageId",
                table: "SmsMessageDetails",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_WriteById",
                table: "SmsMessageDetails",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmsMessageDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "SmsCampaignId",
                table: "SmsConfigs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "SmsAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "SmsAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmsSmss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SmsAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmsMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WriteById = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                        name: "FK_SmsSmss_SmsAccounts_SmsAccountId",
                        column: x => x.SmsAccountId,
                        principalTable: "SmsAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsSmss_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsSmss_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_SmsCampaignId",
                table: "SmsConfigs",
                column: "SmsCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_CreatedById",
                table: "SmsSmss",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_PartnerId",
                table: "SmsSmss",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_SmsAccountId",
                table: "SmsSmss",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_SmsMessageId",
                table: "SmsSmss",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSmss_WriteById",
                table: "SmsSmss",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_SmsCampaign_SmsCampaignId",
                table: "SmsConfigs",
                column: "SmsCampaignId",
                principalTable: "SmsCampaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
