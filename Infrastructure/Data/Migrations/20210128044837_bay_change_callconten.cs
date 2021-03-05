using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class bay_change_callconten : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyCallContents_SurveyUserInputs_UserInputId",
                table: "SurveyCallContents");

            migrationBuilder.DropIndex(
                name: "IX_SurveyCallContents_UserInputId",
                table: "SurveyCallContents");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "SurveyCallContents");

            migrationBuilder.DropColumn(
                name: "UserInputId",
                table: "SurveyCallContents");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignmentId",
                table: "SurveyCallContents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "SurveyCallContents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCallContents_AssignmentId",
                table: "SurveyCallContents",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyCallContents_SurveyAssignments_AssignmentId",
                table: "SurveyCallContents",
                column: "AssignmentId",
                principalTable: "SurveyAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyCallContents_SurveyAssignments_AssignmentId",
                table: "SurveyCallContents");

            migrationBuilder.DropIndex(
                name: "IX_SurveyCallContents_AssignmentId",
                table: "SurveyCallContents");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "SurveyCallContents");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "SurveyCallContents");

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "SurveyCallContents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserInputId",
                table: "SurveyCallContents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCallContents_UserInputId",
                table: "SurveyCallContents",
                column: "UserInputId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyCallContents_SurveyUserInputs_UserInputId",
                table: "SurveyCallContents",
                column: "UserInputId",
                principalTable: "SurveyUserInputs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
