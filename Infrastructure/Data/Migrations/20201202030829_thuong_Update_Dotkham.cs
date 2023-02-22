using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_Update_Dotkham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_DotKhams_DotKhamId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_Routings_RoutingId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_AspNetUsers_UserId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AspNetUsers_AssistantUserId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AccountInvoices_InvoiceId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AspNetUsers_UserId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_DotKhams_DotKhamId",
                table: "DotKhamSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_AccountInvoices_InvoicesId",
                table: "DotKhamSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_Products_ProductId",
                table: "DotKhamSteps");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamSteps_DotKhamId",
                table: "DotKhamSteps");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamSteps_InvoicesId",
                table: "DotKhamSteps");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AssistantUserId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_InvoiceId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_UserId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_RoutingId",
                table: "DotKhamLines");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_UserId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "DotKhamId",
                table: "DotKhamSteps");

            migrationBuilder.DropColumn(
                name: "InvoicesId",
                table: "DotKhamSteps");

            migrationBuilder.DropColumn(
                name: "IsInclude",
                table: "DotKhamSteps");

            migrationBuilder.DropColumn(
                name: "State",
                table: "DotKhamSteps");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "AssistantUserId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "DateFinished",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "RoutingId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "State",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DotKhamLines");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "DotKhamSteps",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountInvoiceId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameStep",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderLineId",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DotKhamLineToothRels",
                columns: table => new
                {
                    LineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhamLineToothRels", x => new { x.LineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_DotKhamLineToothRels_DotKhamLines_LineId",
                        column: x => x.LineId,
                        principalTable: "DotKhamLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DotKhamLineToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AccountInvoiceId",
                table: "DotKhams",
                column: "AccountInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_SaleOrderLineId",
                table: "DotKhamLines",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLineToothRels_ToothId",
                table: "DotKhamLineToothRels",
                column: "ToothId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_DotKhams_DotKhamId",
                table: "DotKhamLines",
                column: "DotKhamId",
                principalTable: "DotKhams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_SaleOrderLines_SaleOrderLineId",
                table: "DotKhamLines",
                column: "SaleOrderLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AccountInvoices_AccountInvoiceId",
                table: "DotKhams",
                column: "AccountInvoiceId",
                principalTable: "AccountInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_Products_ProductId",
                table: "DotKhamSteps",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_DotKhams_DotKhamId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_SaleOrderLines_SaleOrderLineId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AccountInvoices_AccountInvoiceId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_Products_ProductId",
                table: "DotKhamSteps");

            migrationBuilder.DropTable(
                name: "DotKhamLineToothRels");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AccountInvoiceId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_SaleOrderLineId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "AccountInvoiceId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "NameStep",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "SaleOrderLineId",
                table: "DotKhamLines");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "DotKhamSteps",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DotKhamId",
                table: "DotKhamSteps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoicesId",
                table: "DotKhamSteps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInclude",
                table: "DotKhamSteps",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "DotKhamSteps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "DotKhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssistantUserId",
                table: "DotKhams",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "DotKhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "DotKhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DotKhams",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFinished",
                table: "DotKhamLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "DotKhamLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DotKhamLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoutingId",
                table: "DotKhamLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "DotKhamLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DotKhamLines",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_DotKhamId",
                table: "DotKhamSteps",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_InvoicesId",
                table: "DotKhamSteps",
                column: "InvoicesId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantUserId",
                table: "DotKhams",
                column: "AssistantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_InvoiceId",
                table: "DotKhams",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_UserId",
                table: "DotKhams",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_RoutingId",
                table: "DotKhamLines",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_UserId",
                table: "DotKhamLines",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_DotKhams_DotKhamId",
                table: "DotKhamLines",
                column: "DotKhamId",
                principalTable: "DotKhams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_Routings_RoutingId",
                table: "DotKhamLines",
                column: "RoutingId",
                principalTable: "Routings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_AspNetUsers_UserId",
                table: "DotKhamLines",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AspNetUsers_AssistantUserId",
                table: "DotKhams",
                column: "AssistantUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AccountInvoices_InvoiceId",
                table: "DotKhams",
                column: "InvoiceId",
                principalTable: "AccountInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AspNetUsers_UserId",
                table: "DotKhams",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_DotKhams_DotKhamId",
                table: "DotKhamSteps",
                column: "DotKhamId",
                principalTable: "DotKhams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_AccountInvoices_InvoicesId",
                table: "DotKhamSteps",
                column: "InvoicesId",
                principalTable: "AccountInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_Products_ProductId",
                table: "DotKhamSteps",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
