using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Thien_Create_Table_LaboWarranty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LaboWarranty",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    LaboOrderId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: true),
                    DateReceiptWarranty = table.Column<DateTime>(nullable: true),
                    DateSendWarranty = table.Column<DateTime>(nullable: true),
                    DateReceiptInspection = table.Column<DateTime>(nullable: true),
                    DateAssemblyWarranty = table.Column<DateTime>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboWarranty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboWarranty_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboWarranty_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboWarranty_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboWarranty_LaboOrders_LaboOrderId",
                        column: x => x.LaboOrderId,
                        principalTable: "LaboOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboWarranty_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboWarrantyToothRels",
                columns: table => new
                {
                    LaboWarrantyId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboWarrantyToothRels", x => new { x.LaboWarrantyId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_LaboWarrantyToothRels_LaboWarranty_LaboWarrantyId",
                        column: x => x.LaboWarrantyId,
                        principalTable: "LaboWarranty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboWarrantyToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboWarranty_CompanyId",
                table: "LaboWarranty",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboWarranty_CreatedById",
                table: "LaboWarranty",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboWarranty_EmployeeId",
                table: "LaboWarranty",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboWarranty_LaboOrderId",
                table: "LaboWarranty",
                column: "LaboOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboWarranty_WriteById",
                table: "LaboWarranty",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboWarrantyToothRels_ToothId",
                table: "LaboWarrantyToothRels",
                column: "ToothId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaboWarrantyToothRels");

            migrationBuilder.DropTable(
                name: "LaboWarranty");
        }
    }
}
