using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FAddFacebookScheduleAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacebookScheduleAppointmentConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    FBPageId = table.Column<Guid>(nullable: false),
                    ScheduleType = table.Column<string>(nullable: true),
                    ScheduleNumber = table.Column<int>(nullable: true),
                    AutoScheduleAppoint = table.Column<bool>(nullable: false),
                    ContentMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookScheduleAppointmentConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookScheduleAppointmentConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookScheduleAppointmentConfigs_FacebookPages_FBPageId",
                        column: x => x.FBPageId,
                        principalTable: "FacebookPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacebookScheduleAppointmentConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookScheduleAppointmentConfigs_CreatedById",
                table: "FacebookScheduleAppointmentConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookScheduleAppointmentConfigs_FBPageId",
                table: "FacebookScheduleAppointmentConfigs",
                column: "FBPageId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookScheduleAppointmentConfigs_WriteById",
                table: "FacebookScheduleAppointmentConfigs",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookScheduleAppointmentConfigs");
        }
    }
}
