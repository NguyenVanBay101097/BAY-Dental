using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResModel",
                table: "SmsMessages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmsMessageAppointmentRels",
                columns: table => new
                {
                    SmsMessageId = table.Column<Guid>(nullable: false),
                    AppointmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessageAppointmentRels", x => new { x.AppointmentId, x.SmsMessageId });
                    table.ForeignKey(
                        name: "FK_SmsMessageAppointmentRels_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsMessageAppointmentRels_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmsMessageSaleOrderLineRels",
                columns: table => new
                {
                    SmsMessageId = table.Column<Guid>(nullable: false),
                    SaleOrderLineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessageSaleOrderLineRels", x => new { x.SaleOrderLineId, x.SmsMessageId });
                    table.ForeignKey(
                        name: "FK_SmsMessageSaleOrderLineRels_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsMessageSaleOrderLineRels_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmsMessageSaleOrderRels",
                columns: table => new
                {
                    SmsMessageId = table.Column<Guid>(nullable: false),
                    SaleOrderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsMessageSaleOrderRels", x => new { x.SaleOrderId, x.SmsMessageId });
                    table.ForeignKey(
                        name: "FK_SmsMessageSaleOrderRels_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsMessageSaleOrderRels_SmsMessages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalTable: "SmsMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageAppointmentRels_SmsMessageId",
                table: "SmsMessageAppointmentRels",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageSaleOrderLineRels_SmsMessageId",
                table: "SmsMessageSaleOrderLineRels",
                column: "SmsMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageSaleOrderRels_SmsMessageId",
                table: "SmsMessageSaleOrderRels",
                column: "SmsMessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmsMessageAppointmentRels");

            migrationBuilder.DropTable(
                name: "SmsMessageSaleOrderLineRels");

            migrationBuilder.DropTable(
                name: "SmsMessageSaleOrderRels");

            migrationBuilder.DropColumn(
                name: "ResModel",
                table: "SmsMessages");
        }
    }
}
