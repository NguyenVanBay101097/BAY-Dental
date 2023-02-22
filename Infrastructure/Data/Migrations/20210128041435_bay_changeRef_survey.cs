using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class bay_changeRef_survey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyUserInputs_SurveyAssignments_SurveyAssignmentId",
                table: "SurveyUserInputs");

            migrationBuilder.DropIndex(
                name: "IX_SurveyUserInputs_SurveyAssignmentId",
                table: "SurveyUserInputs");

            migrationBuilder.DropColumn(
                name: "SurveyAssignmentId",
                table: "SurveyUserInputs");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompleteDate",
                table: "SurveyAssignments",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserInputId",
                table: "SurveyAssignments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_UserInputId",
                table: "SurveyAssignments",
                column: "UserInputId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAssignments_SurveyUserInputs_UserInputId",
                table: "SurveyAssignments",
                column: "UserInputId",
                principalTable: "SurveyUserInputs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAssignments_SurveyUserInputs_UserInputId",
                table: "SurveyAssignments");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAssignments_UserInputId",
                table: "SurveyAssignments");

            migrationBuilder.DropColumn(
                name: "CompleteDate",
                table: "SurveyAssignments");

            migrationBuilder.DropColumn(
                name: "UserInputId",
                table: "SurveyAssignments");

            migrationBuilder.AddColumn<Guid>(
                name: "SurveyAssignmentId",
                table: "SurveyUserInputs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SurveyUserInputs_SurveyAssignmentId",
                table: "SurveyUserInputs",
                column: "SurveyAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyUserInputs_SurveyAssignments_SurveyAssignmentId",
                table: "SurveyUserInputs",
                column: "SurveyAssignmentId",
                principalTable: "SurveyAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
