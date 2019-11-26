using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F89 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailMessageResPartnerRels",
                columns: table => new
                {
                    MailMessageId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessageResPartnerRels", x => new { x.MailMessageId, x.PartnerId });
                    table.ForeignKey(
                        name: "FK_MailMessageResPartnerRels_MailMessages_MailMessageId",
                        column: x => x.MailMessageId,
                        principalTable: "MailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailMessageResPartnerRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MailNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    MailMessageId = table.Column<Guid>(nullable: false),
                    ResPartnerId = table.Column<Guid>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailNotifications_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailNotifications_MailMessages_MailMessageId",
                        column: x => x.MailMessageId,
                        principalTable: "MailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailNotifications_Partners_ResPartnerId",
                        column: x => x.ResPartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailNotifications_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailMessageResPartnerRels_PartnerId",
                table: "MailMessageResPartnerRels",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotifications_CreatedById",
                table: "MailNotifications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotifications_MailMessageId",
                table: "MailNotifications",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotifications_ResPartnerId",
                table: "MailNotifications",
                column: "ResPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MailNotifications_WriteById",
                table: "MailNotifications",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailMessageResPartnerRels");

            migrationBuilder.DropTable(
                name: "MailNotifications");
        }
    }
}
