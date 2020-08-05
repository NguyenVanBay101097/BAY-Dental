using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTableChamCong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChamCongs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    EmployeeId = table.Column<Guid>(nullable: false),
                    TimeIn = table.Column<DateTime>(nullable: true),
                    TimeOut = table.Column<DateTime>(nullable: true),
                    HourWorked = table.Column<decimal>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChamCongs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChamCongs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChamCongs_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChamCongs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChamCongs_CompanyId",
                table: "ChamCongs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChamCongs_CreatedById",
                table: "ChamCongs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChamCongs_EmployeeId",
                table: "ChamCongs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChamCongs_WriteById",
                table: "ChamCongs",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChamCongs");
        }
    }
}
