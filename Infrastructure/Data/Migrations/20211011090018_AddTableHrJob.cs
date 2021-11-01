using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTableHrJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HrJobId",
                table: "Employees",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HrJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrJobs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrJobs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrJobs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_HrJobId",
                table: "Employees",
                column: "HrJobId");

            migrationBuilder.CreateIndex(
                name: "IX_HrJobs_CompanyId",
                table: "HrJobs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HrJobs_CreatedById",
                table: "HrJobs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrJobs_WriteById",
                table: "HrJobs",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_HrJobs_HrJobId",
                table: "Employees",
                column: "HrJobId",
                principalTable: "HrJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_HrJobs_HrJobId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "HrJobs");

            migrationBuilder.DropIndex(
                name: "IX_Employees_HrJobId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "HrJobId",
                table: "Employees");
        }
    }
}
