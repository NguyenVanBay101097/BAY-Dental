using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateTableBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountHolder",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankId",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Classify",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId1",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Agents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Banks",
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
                    table.PrimaryKey("PK_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Banks_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_BankId",
                table: "Agents",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_CustomerId1",
                table: "Agents",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_EmployeeId",
                table: "Agents",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_CreatedById",
                table: "Banks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_WriteById",
                table: "Banks",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Banks_BankId",
                table: "Agents",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Partners_CustomerId1",
                table: "Agents",
                column: "CustomerId1",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Employees_EmployeeId",
                table: "Agents",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Banks_BankId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Partners_CustomerId1",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Employees_EmployeeId",
                table: "Agents");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropIndex(
                name: "IX_Agents_BankId",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agents_CustomerId1",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agents_EmployeeId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "AccountHolder",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Classify",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Agents");
        }
    }
}
