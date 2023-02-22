using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_update_userinputline_config : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_surveyUserInputLines_SurveyAnswers_AnswerId",
                table: "surveyUserInputLines");

            migrationBuilder.AddForeignKey(
                name: "FK_surveyUserInputLines_SurveyAnswers_AnswerId",
                table: "surveyUserInputLines",
                column: "AnswerId",
                principalTable: "SurveyAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_surveyUserInputLines_SurveyAnswers_AnswerId",
                table: "surveyUserInputLines");

            migrationBuilder.AddForeignKey(
                name: "FK_surveyUserInputLines_SurveyAnswers_AnswerId",
                table: "surveyUserInputLines",
                column: "AnswerId",
                principalTable: "SurveyAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
