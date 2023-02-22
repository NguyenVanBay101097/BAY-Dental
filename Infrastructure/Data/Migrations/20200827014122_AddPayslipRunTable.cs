using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddPayslipRunTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PayslipRunId",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HrPayslipRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: false),
                    DateEnd = table.Column<DateTime>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPayslipRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPayslipRuns_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslipRuns_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslipRuns_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_PayslipRunId",
                table: "HrPayslips",
                column: "PayslipRunId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipRuns_CompanyId",
                table: "HrPayslipRuns",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipRuns_CreatedById",
                table: "HrPayslipRuns",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipRuns_WriteById",
                table: "HrPayslipRuns",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslips_HrPayslipRuns_PayslipRunId",
                table: "HrPayslips",
                column: "PayslipRunId",
                principalTable: "HrPayslipRuns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslips_HrPayslipRuns_PayslipRunId",
                table: "HrPayslips");

            migrationBuilder.DropTable(
                name: "HrPayslipRuns");

            migrationBuilder.DropIndex(
                name: "IX_HrPayslips_PayslipRunId",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "PayslipRunId",
                table: "HrPayslips");
        }
    }
}
