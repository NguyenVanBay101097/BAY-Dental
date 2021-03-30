using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateAdvisory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Advisory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advisory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advisory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advisory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advisory_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToothDiagnosis",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToothDiagnosis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToothDiagnosis_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToothDiagnosis_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvisoryProductRels",
                columns: table => new
                {
                    AdvisoryId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisoryProductRels", x => new { x.AdvisoryId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_AdvisoryProductRels_Advisory_AdvisoryId",
                        column: x => x.AdvisoryId,
                        principalTable: "Advisory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvisoryProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvisoryToothRels",
                columns: table => new
                {
                    AdvisoryId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisoryToothRels", x => new { x.AdvisoryId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_AdvisoryToothRels_Advisory_AdvisoryId",
                        column: x => x.AdvisoryId,
                        principalTable: "Advisory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvisoryToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvisoryToothDiagnosisRels",
                columns: table => new
                {
                    AdvisoryId = table.Column<Guid>(nullable: false),
                    ToothDiagnosisId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisoryToothDiagnosisRels", x => new { x.AdvisoryId, x.ToothDiagnosisId });
                    table.ForeignKey(
                        name: "FK_AdvisoryToothDiagnosisRels_Advisory_AdvisoryId",
                        column: x => x.AdvisoryId,
                        principalTable: "Advisory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvisoryToothDiagnosisRels_ToothDiagnosis_ToothDiagnosisId",
                        column: x => x.ToothDiagnosisId,
                        principalTable: "ToothDiagnosis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_CreatedById",
                table: "Advisory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_UserId",
                table: "Advisory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_WriteById",
                table: "Advisory",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisoryProductRels_ProductId",
                table: "AdvisoryProductRels",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisoryToothDiagnosisRels_ToothDiagnosisId",
                table: "AdvisoryToothDiagnosisRels",
                column: "ToothDiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisoryToothRels_ToothId",
                table: "AdvisoryToothRels",
                column: "ToothId");

            migrationBuilder.CreateIndex(
                name: "IX_ToothDiagnosis_CreatedById",
                table: "ToothDiagnosis",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToothDiagnosis_WriteById",
                table: "ToothDiagnosis",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvisoryProductRels");

            migrationBuilder.DropTable(
                name: "AdvisoryToothDiagnosisRels");

            migrationBuilder.DropTable(
                name: "AdvisoryToothRels");

            migrationBuilder.DropTable(
                name: "ToothDiagnosis");

            migrationBuilder.DropTable(
                name: "Advisory");
        }
    }
}
