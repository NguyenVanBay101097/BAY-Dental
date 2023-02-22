using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class bayadd_partner_to_assignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "SurveyAssignments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_PartnerId",
                table: "SurveyAssignments",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAssignments_Partners_PartnerId",
                table: "SurveyAssignments",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAssignments_Partners_PartnerId",
                table: "SurveyAssignments");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAssignments_PartnerId",
                table: "SurveyAssignments");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "SurveyAssignments");
        }
    }
}
