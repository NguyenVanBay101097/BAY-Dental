using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateSamplePrescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.CreateTable(
                name: "SamplePrescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamplePrescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SamplePrescriptions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SamplePrescriptions_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SamplePrescriptionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PrescriptionId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: true),
                    NumberOfTimes = table.Column<int>(nullable: false),
                    NumberOfDays = table.Column<int>(nullable: false),
                    AmountOfTimes = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    UseAt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamplePrescriptionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SamplePrescriptionLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SamplePrescriptionLines_SamplePrescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "SamplePrescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SamplePrescriptionLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SamplePrescriptionLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SamplePrescriptionLines_CreatedById",
                table: "SamplePrescriptionLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePrescriptionLines_PrescriptionId",
                table: "SamplePrescriptionLines",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePrescriptionLines_ProductId",
                table: "SamplePrescriptionLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePrescriptionLines_WriteById",
                table: "SamplePrescriptionLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePrescriptions_CreatedById",
                table: "SamplePrescriptions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePrescriptions_WriteById",
                table: "SamplePrescriptions",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SamplePrescriptionLines");

            migrationBuilder.DropTable(
                name: "SamplePrescriptions");

          
        }
    }
}
