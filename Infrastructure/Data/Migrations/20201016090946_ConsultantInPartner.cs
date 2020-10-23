using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class ConsultantInPartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConsultantId",
                table: "Partners",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ConsultantId",
                table: "Partners",
                column: "ConsultantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Employees_ConsultantId",
                table: "Partners",
                column: "ConsultantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Employees_ConsultantId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ConsultantId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ConsultantId",
                table: "Partners");
        }
    }
}
