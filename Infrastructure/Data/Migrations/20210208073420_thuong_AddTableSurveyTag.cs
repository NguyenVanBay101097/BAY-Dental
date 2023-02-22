using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddTableSurveyTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateDone",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SurveyTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Color = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyTags_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyTags_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyUserInputSurveyTagRels",
                columns: table => new
                {
                    UserInputId = table.Column<Guid>(nullable: false),
                    SurveyTagId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyUserInputSurveyTagRels", x => new { x.UserInputId, x.SurveyTagId });
                    table.ForeignKey(
                        name: "FK_SurveyUserInputSurveyTagRels_SurveyTags_SurveyTagId",
                        column: x => x.SurveyTagId,
                        principalTable: "SurveyTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurveyUserInputSurveyTagRels_SurveyUserInputs_UserInputId",
                        column: x => x.UserInputId,
                        principalTable: "SurveyUserInputs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyTags_CreatedById",
                table: "SurveyTags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyTags_WriteById",
                table: "SurveyTags",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyUserInputSurveyTagRels_SurveyTagId",
                table: "SurveyUserInputSurveyTagRels",
                column: "SurveyTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyUserInputSurveyTagRels");

            migrationBuilder.DropTable(
                name: "SurveyTags");

            migrationBuilder.DropColumn(
                name: "DateDone",
                table: "SaleOrders");
        }
    }
}
