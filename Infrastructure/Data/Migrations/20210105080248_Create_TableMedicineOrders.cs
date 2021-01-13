using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Create_TableMedicineOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicineOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    JournalId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    ToathuocId = table.Column<Guid>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineOrders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineOrders_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineOrders_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineOrders_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineOrders_ToaThuocs_ToathuocId",
                        column: x => x.ToathuocId,
                        principalTable: "ToaThuocs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineOrders_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicineOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    AmountTotal = table.Column<decimal>(nullable: false),
                    MedicineOrderId = table.Column<Guid>(nullable: false),
                    ToaThuocLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineOrderLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineOrderLines_MedicineOrders_MedicineOrderId",
                        column: x => x.MedicineOrderId,
                        principalTable: "MedicineOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineOrderLines_ToaThuocLines_ToaThuocLineId",
                        column: x => x.ToaThuocLineId,
                        principalTable: "ToaThuocLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineOrderLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderLines_CreatedById",
                table: "MedicineOrderLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderLines_MedicineOrderId",
                table: "MedicineOrderLines",
                column: "MedicineOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderLines_ToaThuocLineId",
                table: "MedicineOrderLines",
                column: "ToaThuocLineId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderLines_WriteById",
                table: "MedicineOrderLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_CompanyId",
                table: "MedicineOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_CreatedById",
                table: "MedicineOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_EmployeeId",
                table: "MedicineOrders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_JournalId",
                table: "MedicineOrders",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_PartnerId",
                table: "MedicineOrders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_ToathuocId",
                table: "MedicineOrders",
                column: "ToathuocId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_WriteById",
                table: "MedicineOrders",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicineOrderLines");

            migrationBuilder.DropTable(
                name: "MedicineOrders");
        }
    }
}
