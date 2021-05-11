using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddProductAppointmentRelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeExpected",
                table: "Appointments",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductAppointmentRels",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    AppoinmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAppointmentRels", x => new { x.ProductId, x.AppoinmentId });
                    table.ForeignKey(
                        name: "FK_ProductAppointmentRels_Appointments_AppoinmentId",
                        column: x => x.AppoinmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAppointmentRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAppointmentRels_AppoinmentId",
                table: "ProductAppointmentRels",
                column: "AppoinmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAppointmentRels");

            migrationBuilder.DropColumn(
                name: "TimeExpected",
                table: "Appointments");
        }
    }
}
