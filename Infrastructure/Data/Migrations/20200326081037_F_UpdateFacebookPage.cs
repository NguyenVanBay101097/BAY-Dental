using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F_UpdateFacebookPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacebookScheduleAppointmentConfigs_FacebookPages_FBPageId",
                table: "FacebookScheduleAppointmentConfigs");

            migrationBuilder.DropIndex(
                name: "IX_FacebookScheduleAppointmentConfigs_FBPageId",
                table: "FacebookScheduleAppointmentConfigs");

            migrationBuilder.DropColumn(
                name: "FBPageId",
                table: "FacebookScheduleAppointmentConfigs");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "FacebookScheduleAppointmentConfigs");

            migrationBuilder.AddColumn<string>(
                name: "RecurringJobId",
                table: "FacebookScheduleAppointmentConfigs",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AutoConfigId",
                table: "FacebookPages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPages_AutoConfigId",
                table: "FacebookPages",
                column: "AutoConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacebookPages_FacebookScheduleAppointmentConfigs_AutoConfigId",
                table: "FacebookPages",
                column: "AutoConfigId",
                principalTable: "FacebookScheduleAppointmentConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacebookPages_FacebookScheduleAppointmentConfigs_AutoConfigId",
                table: "FacebookPages");

            migrationBuilder.DropIndex(
                name: "IX_FacebookPages_AutoConfigId",
                table: "FacebookPages");

            migrationBuilder.DropColumn(
                name: "RecurringJobId",
                table: "FacebookScheduleAppointmentConfigs");

            migrationBuilder.DropColumn(
                name: "AutoConfigId",
                table: "FacebookPages");

            migrationBuilder.AddColumn<Guid>(
                name: "FBPageId",
                table: "FacebookScheduleAppointmentConfigs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "FacebookScheduleAppointmentConfigs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacebookScheduleAppointmentConfigs_FBPageId",
                table: "FacebookScheduleAppointmentConfigs",
                column: "FBPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacebookScheduleAppointmentConfigs_FacebookPages_FBPageId",
                table: "FacebookScheduleAppointmentConfigs",
                column: "FBPageId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
