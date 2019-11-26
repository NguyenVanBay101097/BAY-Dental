using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F90 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentMailMessageRels",
                columns: table => new
                {
                    AppointmentId = table.Column<Guid>(nullable: false),
                    MailMessageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentMailMessageRels", x => new { x.AppointmentId, x.MailMessageId });
                    table.ForeignKey(
                        name: "FK_AppointmentMailMessageRels_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentMailMessageRels_MailMessages_MailMessageId",
                        column: x => x.MailMessageId,
                        principalTable: "MailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMailMessageRels_MailMessageId",
                table: "AppointmentMailMessageRels",
                column: "MailMessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentMailMessageRels");
        }
    }
}
