using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F59 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AppointmentId",
                table: "DotKhams",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Appointments_AppointmentId",
                table: "DotKhams",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Appointments_AppointmentId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AppointmentId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "DotKhams");
        }
    }
}
