using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F77 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrderLines_DotKhams_DotKhamId",
                table: "LaboOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrderLines_AccountInvoices_InvoiceId",
                table: "LaboOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrderLines_Partners_SupplierId",
                table: "LaboOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrderLines_DotKhamId",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "DotKhamId",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "SentDate",
                table: "LaboOrderLines");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "LaboOrderLines",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "LaboOrderLines",
                newName: "ProductQty");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "LaboOrderLines",
                newName: "PartnerId");

            migrationBuilder.RenameIndex(
                name: "IX_LaboOrderLines_SupplierId",
                table: "LaboOrderLines",
                newName: "IX_LaboOrderLines_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_LaboOrderLines_InvoiceId",
                table: "LaboOrderLines",
                newName: "IX_LaboOrderLines_PartnerId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "LaboOrderLines",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "LaboOrderLines",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "LaboOrderLines",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<decimal>(
                name: "PriceTotal",
                table: "LaboOrderLines",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "LaboOrderLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    PartnerRef = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true),
                    DateOrder = table.Column<DateTime>(nullable: false),
                    AmountTotal = table.Column<decimal>(nullable: false),
                    DatePlanned = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    DotKhamId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboOrders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrders_Partners_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrders_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrders_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboOrders_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_CompanyId",
                table: "LaboOrders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_CreatedById",
                table: "LaboOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_CustomerId",
                table: "LaboOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_DotKhamId",
                table: "LaboOrders",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_PartnerId",
                table: "LaboOrders",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_UserId",
                table: "LaboOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_WriteById",
                table: "LaboOrders",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_LaboOrders_OrderId",
                table: "LaboOrderLines",
                column: "OrderId",
                principalTable: "LaboOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_Partners_PartnerId",
                table: "LaboOrderLines",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrderLines_LaboOrders_OrderId",
                table: "LaboOrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrderLines_Partners_PartnerId",
                table: "LaboOrderLines");

            migrationBuilder.DropTable(
                name: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "PriceTotal",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "LaboOrderLines");

            migrationBuilder.RenameColumn(
                name: "ProductQty",
                table: "LaboOrderLines",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "PartnerId",
                table: "LaboOrderLines",
                newName: "InvoiceId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "LaboOrderLines",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_LaboOrderLines_PartnerId",
                table: "LaboOrderLines",
                newName: "IX_LaboOrderLines_InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_LaboOrderLines_OrderId",
                table: "LaboOrderLines",
                newName: "IX_LaboOrderLines_SupplierId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "LaboOrderLines",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "LaboOrderLines",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "LaboOrderLines",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DotKhamId",
                table: "LaboOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "LaboOrderLines",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDate",
                table: "LaboOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_DotKhamId",
                table: "LaboOrderLines",
                column: "DotKhamId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_DotKhams_DotKhamId",
                table: "LaboOrderLines",
                column: "DotKhamId",
                principalTable: "DotKhams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_AccountInvoices_InvoiceId",
                table: "LaboOrderLines",
                column: "InvoiceId",
                principalTable: "AccountInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_Partners_SupplierId",
                table: "LaboOrderLines",
                column: "SupplierId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
