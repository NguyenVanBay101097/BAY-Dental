using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateMailMessageSubtypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubtypeId",
                table: "MailMessages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MailMessageSubtypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailMessageSubtypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailMessageSubtypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailMessageSubtypes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailMessages_SubtypeId",
                table: "MailMessages",
                column: "SubtypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessageSubtypes_CreatedById",
                table: "MailMessageSubtypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MailMessageSubtypes_WriteById",
                table: "MailMessageSubtypes",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_MailMessages_MailMessageSubtypes_SubtypeId",
                table: "MailMessages",
                column: "SubtypeId",
                principalTable: "MailMessageSubtypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailMessages_MailMessageSubtypes_SubtypeId",
                table: "MailMessages");

            migrationBuilder.DropTable(
                name: "MailMessageSubtypes");

            migrationBuilder.DropIndex(
                name: "IX_MailMessages_SubtypeId",
                table: "MailMessages");

            migrationBuilder.DropColumn(
                name: "SubtypeId",
                table: "MailMessages");
        }
    }
}
