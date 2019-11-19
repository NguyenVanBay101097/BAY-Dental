using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F88 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    ResId = table.Column<Guid>(nullable: true),
                    RecordName = table.Column<string>(nullable: true),
                    MessageType = table.Column<string>(nullable: false),
                    EmailFrom = table.Column<string>(nullable: true),
                    AuthorId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailMessages_Partners_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailMessages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailMessages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MailTrackingValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Field = table.Column<string>(nullable: false),
                    FieldDesc = table.Column<string>(nullable: false),
                    FieldType = table.Column<string>(nullable: true),
                    OldValueInteger = table.Column<int>(nullable: true),
                    OldValueDicimal = table.Column<decimal>(nullable: true),
                    OldValueText = table.Column<string>(nullable: true),
                    OldValueDateTime = table.Column<DateTime>(nullable: true),
                    NewValueInteger = table.Column<int>(nullable: true),
                    NewValueDecimal = table.Column<decimal>(nullable: true),
                    NewValueText = table.Column<string>(nullable: true),
                    NewValueDateTime = table.Column<DateTime>(nullable: true),
                    MailMessageId = table.Column<Guid>(nullable: false),
                    TrackSequence = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTrackingValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailTrackingValues_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailTrackingValues_MailMessages_MailMessageId",
                        column: x => x.MailMessageId,
                        principalTable: "MailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailTrackingValues_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_AuthorId",
                table: "MailMessages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_CreatedById",
                table: "MailMessages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_WriteById",
                table: "MailMessages",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_MailTrackingValues_CreatedById",
                table: "MailTrackingValues",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MailTrackingValues_MailMessageId",
                table: "MailTrackingValues",
                column: "MailMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MailTrackingValues_WriteById",
                table: "MailTrackingValues",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailTrackingValues");

            migrationBuilder.DropTable(
                name: "MailMessages");
        }
    }
}
