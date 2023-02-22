using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_AddtableWorkEntyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkEntryTypeId",
                table: "ChamCongs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkEntryTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsHasTimeKeeping = table.Column<bool>(nullable: false),
                    Color = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkEntryTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkEntryTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkEntryTypes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChamCongs_WorkEntryTypeId",
                table: "ChamCongs",
                column: "WorkEntryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkEntryTypes_CreatedById",
                table: "WorkEntryTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkEntryTypes_WriteById",
                table: "WorkEntryTypes",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_ChamCongs_WorkEntryTypes_WorkEntryTypeId",
                table: "ChamCongs",
                column: "WorkEntryTypeId",
                principalTable: "WorkEntryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChamCongs_WorkEntryTypes_WorkEntryTypeId",
                table: "ChamCongs");

            migrationBuilder.DropTable(
                name: "WorkEntryTypes");

            migrationBuilder.DropIndex(
                name: "IX_ChamCongs_WorkEntryTypeId",
                table: "ChamCongs");

            migrationBuilder.DropColumn(
                name: "WorkEntryTypeId",
                table: "ChamCongs");
        }
    }
}
