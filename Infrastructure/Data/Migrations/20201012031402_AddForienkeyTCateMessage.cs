using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddForienkeyTCateMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntervalNumber",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "IntervalType",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "MethodType",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "SheduleDate",
                table: "TCareMessagings");

            migrationBuilder.AddColumn<Guid>(
                name: "TCareMessagingId",
                table: "TCareMessingTraces",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountPartner",
                table: "TCareMessagings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TCareMessagingId",
                table: "TCareMessages",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TCareMessagingTraceId",
                table: "TCareMessages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessingTraces_TCareMessagingId",
                table: "TCareMessingTraces",
                column: "TCareMessagingId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_TCareMessagingId",
                table: "TCareMessages",
                column: "TCareMessagingId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessages_TCareMessagingTraceId",
                table: "TCareMessages",
                column: "TCareMessagingTraceId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessages_TCareMessagings_TCareMessagingId",
                table: "TCareMessages",
                column: "TCareMessagingId",
                principalTable: "TCareMessagings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessages_TCareMessingTraces_TCareMessagingTraceId",
                table: "TCareMessages",
                column: "TCareMessagingTraceId",
                principalTable: "TCareMessingTraces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessingTraces_TCareMessagings_TCareMessagingId",
                table: "TCareMessingTraces",
                column: "TCareMessagingId",
                principalTable: "TCareMessagings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessages_TCareMessagings_TCareMessagingId",
                table: "TCareMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessages_TCareMessingTraces_TCareMessagingTraceId",
                table: "TCareMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessingTraces_TCareMessagings_TCareMessagingId",
                table: "TCareMessingTraces");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessingTraces_TCareMessagingId",
                table: "TCareMessingTraces");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessages_TCareMessagingId",
                table: "TCareMessages");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessages_TCareMessagingTraceId",
                table: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "TCareMessagingId",
                table: "TCareMessingTraces");

            migrationBuilder.DropColumn(
                name: "CountPartner",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "TCareMessagingId",
                table: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "TCareMessagingTraceId",
                table: "TCareMessages");

            migrationBuilder.AddColumn<int>(
                name: "IntervalNumber",
                table: "TCareMessagings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntervalType",
                table: "TCareMessagings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MethodType",
                table: "TCareMessagings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SheduleDate",
                table: "TCareMessagings",
                type: "datetime2",
                nullable: true);
        }
    }
}
