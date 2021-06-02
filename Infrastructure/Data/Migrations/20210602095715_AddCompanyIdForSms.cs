using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddCompanyIdForSms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "SmsTemplates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "SmsMessages",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "SmsMessageDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplates_CompanyId",
                table: "SmsTemplates",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_CompanyId",
                table: "SmsMessages",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_CompanyId",
                table: "SmsMessageDetails",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsMessageDetails_Companies_CompanyId",
                table: "SmsMessageDetails",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsMessages_Companies_CompanyId",
                table: "SmsMessages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsTemplates_Companies_CompanyId",
                table: "SmsTemplates",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsMessageDetails_Companies_CompanyId",
                table: "SmsMessageDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsMessages_Companies_CompanyId",
                table: "SmsMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsTemplates_Companies_CompanyId",
                table: "SmsTemplates");

            migrationBuilder.DropIndex(
                name: "IX_SmsTemplates_CompanyId",
                table: "SmsTemplates");

            migrationBuilder.DropIndex(
                name: "IX_SmsMessages_CompanyId",
                table: "SmsMessages");

            migrationBuilder.DropIndex(
                name: "IX_SmsMessageDetails_CompanyId",
                table: "SmsMessageDetails");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SmsTemplates");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SmsMessages");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SmsMessageDetails");
        }
    }
}
