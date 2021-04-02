using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateToothDiagnosisProductRel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvisoryToothDiagnosisRels_Advisory_AdvisoryId",
                table: "AdvisoryToothDiagnosisRels");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvisoryToothDiagnosisRels_ToothDiagnosis_ToothDiagnosisId",
                table: "AdvisoryToothDiagnosisRels");

            migrationBuilder.CreateTable(
                name: "ToothDiagnosisProductRels",
                columns: table => new
                {
                    ToothDiagnosisId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToothDiagnosisProductRels", x => new { x.ToothDiagnosisId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ToothDiagnosisProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToothDiagnosisProductRels_ToothDiagnosis_ToothDiagnosisId",
                        column: x => x.ToothDiagnosisId,
                        principalTable: "ToothDiagnosis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToothDiagnosisProductRels_ProductId",
                table: "ToothDiagnosisProductRels",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisoryToothDiagnosisRels_Advisory_AdvisoryId",
                table: "AdvisoryToothDiagnosisRels",
                column: "AdvisoryId",
                principalTable: "Advisory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisoryToothDiagnosisRels_ToothDiagnosis_ToothDiagnosisId",
                table: "AdvisoryToothDiagnosisRels",
                column: "ToothDiagnosisId",
                principalTable: "ToothDiagnosis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvisoryToothDiagnosisRels_Advisory_AdvisoryId",
                table: "AdvisoryToothDiagnosisRels");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvisoryToothDiagnosisRels_ToothDiagnosis_ToothDiagnosisId",
                table: "AdvisoryToothDiagnosisRels");

            migrationBuilder.DropTable(
                name: "ToothDiagnosisProductRels");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisoryToothDiagnosisRels_Advisory_AdvisoryId",
                table: "AdvisoryToothDiagnosisRels",
                column: "AdvisoryId",
                principalTable: "Advisory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisoryToothDiagnosisRels_ToothDiagnosis_ToothDiagnosisId",
                table: "AdvisoryToothDiagnosisRels",
                column: "ToothDiagnosisId",
                principalTable: "ToothDiagnosis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
