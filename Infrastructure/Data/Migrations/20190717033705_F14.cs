using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DotKhams",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    DoctorId = table.Column<Guid>(nullable: false),
                    AssistantId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DotKhams_Partners_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhams_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhams_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhams_Partners_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhams_AccountInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhams_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhams_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhams_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DotKhamLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    DotKhamId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhamLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DotKhamLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamLines_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DotKhamLineOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    LineId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhamLineOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DotKhamLineOperations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamLineOperations_DotKhamLines_LineId",
                        column: x => x.LineId,
                        principalTable: "DotKhamLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamLineOperations_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineOperations_CreatedById",
                table: "DotKhamLineOperations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineOperations_LineId",
                table: "DotKhamLineOperations",
                column: "LineId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineOperations_WriteById",
                table: "DotKhamLineOperations",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_CreatedById",
                table: "DotKhamLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_DotKhamId",
                table: "DotKhamLines",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_ProductId",
                table: "DotKhamLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_WriteById",
                table: "DotKhamLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_CompanyId",
                table: "DotKhams",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_CreatedById",
                table: "DotKhams",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_DoctorId",
                table: "DotKhams",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_InvoiceId",
                table: "DotKhams",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_PartnerId",
                table: "DotKhams",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_UserId",
                table: "DotKhams",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_WriteById",
                table: "DotKhams",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DotKhamLineOperations");

            migrationBuilder.DropTable(
                name: "DotKhamLines");

            migrationBuilder.DropTable(
                name: "DotKhams");
        }
    }
}
