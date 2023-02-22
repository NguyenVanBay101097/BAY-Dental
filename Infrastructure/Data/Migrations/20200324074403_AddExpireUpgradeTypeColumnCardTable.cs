using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddExpireUpgradeTypeColumnCardTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UpgradeTypeId",
                table: "CardCards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardCards_UpgradeTypeId",
                table: "CardCards",
                column: "UpgradeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardCards_CardTypes_UpgradeTypeId",
                table: "CardCards",
                column: "UpgradeTypeId",
                principalTable: "CardTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardCards_CardTypes_UpgradeTypeId",
                table: "CardCards");

            migrationBuilder.DropIndex(
                name: "IX_CardCards_UpgradeTypeId",
                table: "CardCards");

            migrationBuilder.DropColumn(
                name: "UpgradeTypeId",
                table: "CardCards");
        }
    }
}
