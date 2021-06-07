using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class ChangeColCompanyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsCampaign_Companies_CompanyId",
                table: "SmsCampaign");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_Companies_CompanyId",
                table: "SmsConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsMessageDetails_Companies_CompanyId",
                table: "SmsMessageDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsMessages_Companies_CompanyId",
                table: "SmsMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsTemplates_Companies_CompanyId",
                table: "SmsTemplates");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsTemplates",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsMessages",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsMessageDetails",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsConfigs",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsCampaign",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsCampaign_Companies_CompanyId",
                table: "SmsCampaign",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_Companies_CompanyId",
                table: "SmsConfigs",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsMessageDetails_Companies_CompanyId",
                table: "SmsMessageDetails",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsMessages_Companies_CompanyId",
                table: "SmsMessages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsTemplates_Companies_CompanyId",
                table: "SmsTemplates",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsCampaign_Companies_CompanyId",
                table: "SmsCampaign");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_Companies_CompanyId",
                table: "SmsConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsMessageDetails_Companies_CompanyId",
                table: "SmsMessageDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsMessages_Companies_CompanyId",
                table: "SmsMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsTemplates_Companies_CompanyId",
                table: "SmsTemplates");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsTemplates",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsMessages",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsMessageDetails",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsConfigs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "SmsCampaign",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsCampaign_Companies_CompanyId",
                table: "SmsCampaign",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_Companies_CompanyId",
                table: "SmsConfigs",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
