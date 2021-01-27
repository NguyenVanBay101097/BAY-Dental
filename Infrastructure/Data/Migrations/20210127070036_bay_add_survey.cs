using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class bay_add_survey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAllowSurvey",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SurveyQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyQuestions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyQuestions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyQuestions_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyUserInputs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Score = table.Column<decimal>(nullable: true),
                    MaxScore = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyUserInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyUserInputs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyUserInputs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    QuestionId = table.Column<Guid>(nullable: false),
                    Score = table.Column<decimal>(nullable: true),
                    Sequence = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyAnswers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyAnswers_SurveyQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "SurveyQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurveyAnswers_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SurveyId = table.Column<Guid>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: false),
                    SaleOrderId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyAssignments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyAssignments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurveyAssignments_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurveyAssignments_SurveyUserInputs_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "SurveyUserInputs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurveyAssignments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "surveyUserInputLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Score = table.Column<decimal>(nullable: true),
                    ValueText = table.Column<string>(nullable: true),
                    UserInputId = table.Column<Guid>(nullable: false),
                    AnswerId = table.Column<Guid>(nullable: true),
                    QuestionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_surveyUserInputLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_surveyUserInputLines_SurveyAnswers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "SurveyAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_surveyUserInputLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_surveyUserInputLines_SurveyQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "SurveyQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_surveyUserInputLines_SurveyUserInputs_UserInputId",
                        column: x => x.UserInputId,
                        principalTable: "SurveyUserInputs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_surveyUserInputLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswers_CreatedById",
                table: "SurveyAnswers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswers_QuestionId",
                table: "SurveyAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswers_WriteById",
                table: "SurveyAnswers",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_CompanyId",
                table: "SurveyAssignments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_CreatedById",
                table: "SurveyAssignments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_EmployeeId",
                table: "SurveyAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_SaleOrderId",
                table: "SurveyAssignments",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_SurveyId",
                table: "SurveyAssignments",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_WriteById",
                table: "SurveyAssignments",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestions_CompanyId",
                table: "SurveyQuestions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestions_CreatedById",
                table: "SurveyQuestions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestions_WriteById",
                table: "SurveyQuestions",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_surveyUserInputLines_AnswerId",
                table: "surveyUserInputLines",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_surveyUserInputLines_CreatedById",
                table: "surveyUserInputLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_surveyUserInputLines_QuestionId",
                table: "surveyUserInputLines",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_surveyUserInputLines_UserInputId",
                table: "surveyUserInputLines",
                column: "UserInputId");

            migrationBuilder.CreateIndex(
                name: "IX_surveyUserInputLines_WriteById",
                table: "surveyUserInputLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyUserInputs_CreatedById",
                table: "SurveyUserInputs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyUserInputs_WriteById",
                table: "SurveyUserInputs",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyAssignments");

            migrationBuilder.DropTable(
                name: "surveyUserInputLines");

            migrationBuilder.DropTable(
                name: "SurveyAnswers");

            migrationBuilder.DropTable(
                name: "SurveyUserInputs");

            migrationBuilder.DropTable(
                name: "SurveyQuestions");

            migrationBuilder.DropColumn(
                name: "IsAllowSurvey",
                table: "Employees");
        }
    }
}
