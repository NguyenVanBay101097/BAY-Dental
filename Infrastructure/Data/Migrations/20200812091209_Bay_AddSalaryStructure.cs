using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_AddSalaryStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HrPayrollStructures",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    SchedulePay = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPayrollStructures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPayrollStructures_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayrollStructures_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayrollStructures_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HrSalaryRuleCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSalaryRuleCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSalaryRuleCategories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrSalaryRuleCategories_HrSalaryRuleCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "HrSalaryRuleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrSalaryRuleCategories_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HrSalaryRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    AmountSelect = table.Column<string>(nullable: true),
                    AmountFix = table.Column<decimal>(nullable: true),
                    AmountPercentage = table.Column<decimal>(nullable: true),
                    AppearsOnPayslip = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    StructId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSalaryRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSalaryRules_HrSalaryRuleCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "HrSalaryRuleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrSalaryRules_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrSalaryRules_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrSalaryRules_HrPayrollStructures_StructId",
                        column: x => x.StructId,
                        principalTable: "HrPayrollStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrSalaryRules_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructures_CompanyId",
                table: "HrPayrollStructures",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructures_CreatedById",
                table: "HrPayrollStructures",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructures_WriteById",
                table: "HrPayrollStructures",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryRuleCategories_CreatedById",
                table: "HrSalaryRuleCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryRuleCategories_ParentId",
                table: "HrSalaryRuleCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryRuleCategories_WriteById",
                table: "HrSalaryRuleCategories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryRules_CategoryId",
                table: "HrSalaryRules",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryRules_CompanyId",
                table: "HrSalaryRules",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryRules_CreatedById",
                table: "HrSalaryRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryRules_StructId",
                table: "HrSalaryRules",
                column: "StructId");

            migrationBuilder.CreateIndex(
                name: "IX_HrSalaryRules_WriteById",
                table: "HrSalaryRules",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrSalaryRules");

            migrationBuilder.DropTable(
                name: "HrSalaryRuleCategories");

            migrationBuilder.DropTable(
                name: "HrPayrollStructures");
        }
    }
}
