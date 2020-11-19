using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateTableSalarypayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalaryPaymentId",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SalaryPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    JournalId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: true),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryPayments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalaryPayments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalaryPayments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalaryPayments_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalaryPayments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PartnerId",
                table: "Employees",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_SalaryPaymentId",
                table: "AccountMoveLines",
                column: "SalaryPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPayments_CompanyId",
                table: "SalaryPayments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPayments_CreatedById",
                table: "SalaryPayments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPayments_EmployeeId",
                table: "SalaryPayments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPayments_JournalId",
                table: "SalaryPayments",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPayments_WriteById",
                table: "SalaryPayments",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_SalaryPayments_SalaryPaymentId",
                table: "AccountMoveLines",
                column: "SalaryPaymentId",
                principalTable: "SalaryPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Partners_PartnerId",
                table: "Employees",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_SalaryPayments_SalaryPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Partners_PartnerId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "SalaryPayments");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PartnerId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_SalaryPaymentId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SalaryPaymentId",
                table: "AccountMoveLines");
        }
    }
}
