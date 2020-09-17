using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddPayslipTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HrPayslips",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    StructId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<Guid>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPayslips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPayslips_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslips_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslips_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslips_HrPayrollStructures_StructId",
                        column: x => x.StructId,
                        principalTable: "HrPayrollStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslips_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HrPayslipLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    Total = table.Column<decimal>(nullable: true),
                    SlipId = table.Column<Guid>(nullable: false),
                    SalaryRuleId = table.Column<Guid>(nullable: false),
                    Rate = table.Column<decimal>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: true),
                    Sequence = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPayslipLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPayslipLines_HrSalaryRuleCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "HrSalaryRuleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslipLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslipLines_HrSalaryRules_SalaryRuleId",
                        column: x => x.SalaryRuleId,
                        principalTable: "HrSalaryRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslipLines_HrPayslips_SlipId",
                        column: x => x.SlipId,
                        principalTable: "HrPayslips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HrPayslipLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HrPayslipWorkedDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PayslipId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    NumberOfDays = table.Column<decimal>(nullable: true),
                    NumberOfHours = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPayslipWorkedDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPayslipWorkedDays_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayslipWorkedDays_HrPayslips_PayslipId",
                        column: x => x.PayslipId,
                        principalTable: "HrPayslips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HrPayslipWorkedDays_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipLines_CategoryId",
                table: "HrPayslipLines",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipLines_CreatedById",
                table: "HrPayslipLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipLines_SalaryRuleId",
                table: "HrPayslipLines",
                column: "SalaryRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipLines_SlipId",
                table: "HrPayslipLines",
                column: "SlipId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipLines_WriteById",
                table: "HrPayslipLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_CompanyId",
                table: "HrPayslips",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_CreatedById",
                table: "HrPayslips",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_EmployeeId",
                table: "HrPayslips",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_StructId",
                table: "HrPayslips",
                column: "StructId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_WriteById",
                table: "HrPayslips",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipWorkedDays_CreatedById",
                table: "HrPayslipWorkedDays",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipWorkedDays_PayslipId",
                table: "HrPayslipWorkedDays",
                column: "PayslipId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipWorkedDays_WriteById",
                table: "HrPayslipWorkedDays",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrPayslipLines");

            migrationBuilder.DropTable(
                name: "HrPayslipWorkedDays");

            migrationBuilder.DropTable(
                name: "HrPayslips");
        }
    }
}
