using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateHangThanhVienTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_MemberLevels_MemberLevelId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_MemberLevelId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "MemberLevelId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Partners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MemberLevelId",
                table: "Partners",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Point",
                table: "Partners",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_MemberLevelId",
                table: "Partners",
                column: "MemberLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_MemberLevels_MemberLevelId",
                table: "Partners",
                column: "MemberLevelId",
                principalTable: "MemberLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
