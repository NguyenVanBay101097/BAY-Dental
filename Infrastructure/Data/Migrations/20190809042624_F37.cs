using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F37 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DotKhamId",
                table: "LaboOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_DotKhamId",
                table: "LaboOrderLines",
                column: "DotKhamId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_DotKhams_DotKhamId",
                table: "LaboOrderLines",
                column: "DotKhamId",
                principalTable: "DotKhams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrderLines_DotKhams_DotKhamId",
                table: "LaboOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrderLines_DotKhamId",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "DotKhamId",
                table: "LaboOrderLines");
        }
    }
}
