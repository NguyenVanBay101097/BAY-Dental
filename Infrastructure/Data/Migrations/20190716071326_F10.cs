using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Diagnostic",
                table: "AccountInvoiceLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "AccountInvoiceLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "AccountInvoiceLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountInvoiceLineToothRel",
                columns: table => new
                {
                    InvoiceLineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvoiceLineToothRel", x => new { x.InvoiceLineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLineToothRel_AccountInvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "AccountInvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountInvoiceLineToothRel_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLines_EmployeeId",
                table: "AccountInvoiceLines",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvoiceLineToothRel_ToothId",
                table: "AccountInvoiceLineToothRel",
                column: "ToothId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_Partners_EmployeeId",
                table: "AccountInvoiceLines",
                column: "EmployeeId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoiceLines_Partners_EmployeeId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropTable(
                name: "AccountInvoiceLineToothRel");

            migrationBuilder.DropIndex(
                name: "IX_AccountInvoiceLines_EmployeeId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropColumn(
                name: "Diagnostic",
                table: "AccountInvoiceLines");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "AccountInvoiceLines");
        }
    }
}
