using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateRuleCardTypeAndCardCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "ServiceCardCards",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "CardTypes",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "CardCards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCardCards_CompanyId",
                table: "ServiceCardCards",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTypes_CompanyId",
                table: "CardTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CardCards_CompanyId",
                table: "CardCards",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardCards_Companies_CompanyId",
                table: "CardCards",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardTypes_Companies_CompanyId",
                table: "CardTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCardCards_Companies_CompanyId",
                table: "ServiceCardCards",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardCards_Companies_CompanyId",
                table: "CardCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CardTypes_Companies_CompanyId",
                table: "CardTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCardCards_Companies_CompanyId",
                table: "ServiceCardCards");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCardCards_CompanyId",
                table: "ServiceCardCards");

            migrationBuilder.DropIndex(
                name: "IX_CardTypes_CompanyId",
                table: "CardTypes");

            migrationBuilder.DropIndex(
                name: "IX_CardCards_CompanyId",
                table: "CardCards");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ServiceCardCards");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CardTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CardCards");
        }
    }
}
