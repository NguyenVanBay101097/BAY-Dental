using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.TenantData.Migrations
{
    public partial class AddEmployeAdminTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupporterName",
                table: "Tenants");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Tenants",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAdmins_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAdmins_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_EmployeeId",
                table: "Tenants",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAdmins_CreatedById",
                table: "EmployeeAdmins",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAdmins_WriteById",
                table: "EmployeeAdmins",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_EmployeeAdmins_EmployeeId",
                table: "Tenants",
                column: "EmployeeId",
                principalTable: "EmployeeAdmins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_EmployeeAdmins_EmployeeId",
                table: "Tenants");

            migrationBuilder.DropTable(
                name: "EmployeeAdmins");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_EmployeeId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "SupporterName",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
