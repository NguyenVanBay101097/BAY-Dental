using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FacebookMassMessagingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacebookMassMessagings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    SentDate = table.Column<DateTime>(nullable: true),
                    ScheduleDate = table.Column<DateTime>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    FacebookPageId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookMassMessagings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookMassMessagings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookMassMessagings_FacebookPages_FacebookPageId",
                        column: x => x.FacebookPageId,
                        principalTable: "FacebookPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FacebookMassMessagings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacebookMessagingTraces",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    MassMessagingId = table.Column<Guid>(nullable: true),
                    Sent = table.Column<DateTime>(nullable: true),
                    Exception = table.Column<DateTime>(nullable: true),
                    MessageId = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookMessagingTraces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookMessagingTraces_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookMessagingTraces_FacebookMassMessagings_MassMessagingId",
                        column: x => x.MassMessagingId,
                        principalTable: "FacebookMassMessagings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacebookMessagingTraces_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookMassMessagings_CreatedById",
                table: "FacebookMassMessagings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookMassMessagings_FacebookPageId",
                table: "FacebookMassMessagings",
                column: "FacebookPageId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookMassMessagings_WriteById",
                table: "FacebookMassMessagings",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookMessagingTraces_CreatedById",
                table: "FacebookMessagingTraces",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookMessagingTraces_MassMessagingId",
                table: "FacebookMessagingTraces",
                column: "MassMessagingId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookMessagingTraces_WriteById",
                table: "FacebookMessagingTraces",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookMessagingTraces");

            migrationBuilder.DropTable(
                name: "FacebookMassMessagings");
        }
    }
}
