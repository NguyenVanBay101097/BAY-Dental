using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F34 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Partners_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Partners_DoctorId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_DoctorId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "DotKhams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "DotKhams",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_DoctorId",
                table: "DotKhams",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Partners_AssistantId",
                table: "DotKhams",
                column: "AssistantId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Partners_DoctorId",
                table: "DotKhams",
                column: "DoctorId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
