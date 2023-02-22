using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddUoMToaThuoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductUoMId",
                table: "ToaThuocLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductUoMId",
                table: "SamplePrescriptionLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductUoMId",
                table: "MedicineOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_ProductUoMId",
                table: "ToaThuocLines",
                column: "ProductUoMId");

            migrationBuilder.CreateIndex(
                name: "IX_SamplePrescriptionLines_ProductUoMId",
                table: "SamplePrescriptionLines",
                column: "ProductUoMId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderLines_ProductUoMId",
                table: "MedicineOrderLines",
                column: "ProductUoMId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrderLines_UoMs_ProductUoMId",
                table: "MedicineOrderLines",
                column: "ProductUoMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SamplePrescriptionLines_UoMs_ProductUoMId",
                table: "SamplePrescriptionLines",
                column: "ProductUoMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ToaThuocLines_UoMs_ProductUoMId",
                table: "ToaThuocLines",
                column: "ProductUoMId",
                principalTable: "UoMs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrderLines_UoMs_ProductUoMId",
                table: "MedicineOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SamplePrescriptionLines_UoMs_ProductUoMId",
                table: "SamplePrescriptionLines");

            migrationBuilder.DropForeignKey(
                name: "FK_ToaThuocLines_UoMs_ProductUoMId",
                table: "ToaThuocLines");

            migrationBuilder.DropIndex(
                name: "IX_ToaThuocLines_ProductUoMId",
                table: "ToaThuocLines");

            migrationBuilder.DropIndex(
                name: "IX_SamplePrescriptionLines_ProductUoMId",
                table: "SamplePrescriptionLines");

            migrationBuilder.DropIndex(
                name: "IX_MedicineOrderLines_ProductUoMId",
                table: "MedicineOrderLines");

            migrationBuilder.DropColumn(
                name: "ProductUoMId",
                table: "ToaThuocLines");

            migrationBuilder.DropColumn(
                name: "ProductUoMId",
                table: "SamplePrescriptionLines");

            migrationBuilder.DropColumn(
                name: "ProductUoMId",
                table: "MedicineOrderLines");
        }
    }
}
