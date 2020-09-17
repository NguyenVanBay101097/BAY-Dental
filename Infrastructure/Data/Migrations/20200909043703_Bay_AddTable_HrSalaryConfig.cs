using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_AddTable_HrSalaryConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "DefaultWorkEntryTypeId",
                table: "HrPayrollStructureTypes",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "HrSalaryConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    DefaultGlobalLeaveTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSalaryConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSalaryConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrSalaryConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrSalaryConfigs_WorkEntryTypes_DefaultGlobalLeaveTypeId",
                        column: x => x.DefaultGlobalLeaveTypeId,
                        principalTable: "WorkEntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HrSalaryConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryConfigs_CompanyId",
                table: "HrSalaryConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryConfigs_CreatedById",
                table: "HrSalaryConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryConfigs_DefaultGlobalLeaveTypeId",
                table: "HrSalaryConfigs",
                column: "DefaultGlobalLeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryConfigs_WriteById",
                table: "HrSalaryConfigs",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrSalaryConfigs");

            migrationBuilder.AlterColumn<Guid>(
                name: "DefaultWorkEntryTypeId",
                table: "HrPayrollStructureTypes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
