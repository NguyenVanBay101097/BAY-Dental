using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_CreateCustomerReceiptTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    DateWaitting = table.Column<DateTime>(nullable: true),
                    DateExamination = table.Column<DateTime>(nullable: true),
                    DateDone = table.Column<DateTime>(nullable: true),
                    TimeExpected = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    DoctorId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    IsRepeatCustomer = table.Column<bool>(nullable: false),
                    IsNoTreatment = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReceipts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReceipts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReceipts_Employees_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReceipts_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReceipts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReceipts_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReceiptProductRels",
                columns: table => new
                {
                    CustomerReceiptId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReceiptProductRels", x => new { x.CustomerReceiptId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_CustomerReceiptProductRels_CustomerReceipts_CustomerReceiptId",
                        column: x => x.CustomerReceiptId,
                        principalTable: "CustomerReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerReceiptProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceiptProductRels_ProductId",
                table: "CustomerReceiptProductRels",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_CompanyId",
                table: "CustomerReceipts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_CreatedById",
                table: "CustomerReceipts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_DoctorId",
                table: "CustomerReceipts",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_PartnerId",
                table: "CustomerReceipts",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_UserId",
                table: "CustomerReceipts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReceipts_WriteById",
                table: "CustomerReceipts",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReceiptProductRels");

            migrationBuilder.DropTable(
                name: "CustomerReceipts");
        }
    }
}
