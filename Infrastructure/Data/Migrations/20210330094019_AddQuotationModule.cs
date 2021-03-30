using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddQuotationModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuotationLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    PercentDiscount = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: true),
                    DoctorId = table.Column<Guid>(nullable: true),
                    AssistantId = table.Column<Guid>(nullable: true),
                    Diagnostic = table.Column<string>(nullable: true),
                    ToothCategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Employees_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Employees_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationLines_ToothCategories_ToothCategoryId",
                        column: x => x.ToothCategoryId,
                        principalTable: "ToothCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    DateQuotation = table.Column<DateTime>(nullable: false),
                    DateApplies = table.Column<int>(nullable: false),
                    DateEndQuotation = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotations_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotations_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationLineToothRels",
                columns: table => new
                {
                    QuotationLineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationLineToothRels", x => new { x.QuotationLineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_QuotationLineToothRels_QuotationLines_QuotationLineId",
                        column: x => x.QuotationLineId,
                        principalTable: "QuotationLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationLineToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentQuotations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    DiscountPercent = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    QuotationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentQuotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentQuotations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentQuotations_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentQuotations_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotations_CreatedById",
                table: "PaymentQuotations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotations_QuotationId",
                table: "PaymentQuotations",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentQuotations_WriteById",
                table: "PaymentQuotations",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AssistantId",
                table: "QuotationLines",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_CreatedById",
                table: "QuotationLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_DoctorId",
                table: "QuotationLines",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_ProductId",
                table: "QuotationLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_ToothCategoryId",
                table: "QuotationLines",
                column: "ToothCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_WriteById",
                table: "QuotationLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLineToothRels_ToothId",
                table: "QuotationLineToothRels",
                column: "ToothId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_CreatedById",
                table: "Quotations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_PartnerId",
                table: "Quotations",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_UserId",
                table: "Quotations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_WriteById",
                table: "Quotations",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentQuotations");

            migrationBuilder.DropTable(
                name: "QuotationLineToothRels");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropTable(
                name: "QuotationLines");
        }
    }
}
