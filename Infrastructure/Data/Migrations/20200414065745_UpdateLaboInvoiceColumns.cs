using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateLaboInvoiceColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LaboLineId",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_LaboLineId",
                table: "AccountMoveLines",
                column: "LaboLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_LaboOrderLines_LaboLineId",
                table: "AccountMoveLines",
                column: "LaboLineId",
                principalTable: "LaboOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_LaboOrderLines_LaboLineId",
                table: "AccountMoveLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_LaboLineId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "LaboLineId",
                table: "AccountMoveLines");
        }
    }
}
