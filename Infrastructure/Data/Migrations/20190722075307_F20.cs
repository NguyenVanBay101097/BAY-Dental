using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Usage = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ParentLocationId = table.Column<Guid>(nullable: true),
                    CompleteName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    ScrapLocation = table.Column<bool>(nullable: false),
                    ParentLeft = table.Column<int>(nullable: true),
                    ParentRight = table.Column<int>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    NameGet = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLocations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLocations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLocations_StockLocations_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLocations_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockMoves",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    DateExpected = table.Column<DateTime>(nullable: true),
                    PickingTypeId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    ProductUOMQty = table.Column<decimal>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    LocationDestId = table.Column<Guid>(nullable: false),
                    WarehouseId = table.Column<Guid>(nullable: true),
                    PickingId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    PriceUnit = table.Column<decimal>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Sequence = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMoves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMoves_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_StockLocations_LocationDestId",
                        column: x => x.LocationDestId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMoves_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockQuants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    InDate = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    NegativeMoveId = table.Column<Guid>(nullable: true),
                    PropagatedFromId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockQuants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockQuants_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_StockMoves_NegativeMoveId",
                        column: x => x.NegativeMoveId,
                        principalTable: "StockMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_StockQuants_PropagatedFromId",
                        column: x => x.PropagatedFromId,
                        principalTable: "StockQuants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuants_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockQuantMoveRel",
                columns: table => new
                {
                    QuantId = table.Column<Guid>(nullable: false),
                    MoveId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockQuantMoveRel", x => new { x.MoveId, x.QuantId });
                    table.ForeignKey(
                        name: "FK_StockQuantMoveRel_StockMoves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "StockMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockQuantMoveRel_StockQuants_QuantId",
                        column: x => x.QuantId,
                        principalTable: "StockQuants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockPickings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    PickingTypeId = table.Column<Guid>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    LocationDestId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPickings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockPickings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_StockLocations_LocationDestId",
                        column: x => x.LocationDestId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockPickingTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    DefaultLocationDestId = table.Column<Guid>(nullable: true),
                    WarehouseId = table.Column<Guid>(nullable: true),
                    IRSequenceId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DefaultLocationSrcId = table.Column<Guid>(nullable: true),
                    ReturnPickingTypeId = table.Column<Guid>(nullable: true),
                    UseCreateLots = table.Column<bool>(nullable: true),
                    UseExistingLots = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPickingTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_StockLocations_DefaultLocationDestId",
                        column: x => x.DefaultLocationDestId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_StockLocations_DefaultLocationSrcId",
                        column: x => x.DefaultLocationSrcId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_IRSequences_IRSequenceId",
                        column: x => x.IRSequenceId,
                        principalTable: "IRSequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_StockPickingTypes_ReturnPickingTypeId",
                        column: x => x.ReturnPickingTypeId,
                        principalTable: "StockPickingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPickingTypes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockWarehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true),
                    ViewLocationId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    InTypeId = table.Column<Guid>(nullable: true),
                    OutTypeId = table.Column<Guid>(nullable: true),
                    ReceptionSteps = table.Column<string>(nullable: true),
                    DeliverySteps = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockWarehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_StockPickingTypes_InTypeId",
                        column: x => x.InTypeId,
                        principalTable: "StockPickingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_StockLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_StockPickingTypes_OutTypeId",
                        column: x => x.OutTypeId,
                        principalTable: "StockPickingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_StockLocations_ViewLocationId",
                        column: x => x.ViewLocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockWarehouses_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_CompanyId",
                table: "StockLocations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_CreatedById",
                table: "StockLocations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_ParentLocationId",
                table: "StockLocations",
                column: "ParentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_WriteById",
                table: "StockLocations",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_CompanyId",
                table: "StockMoves",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_CreatedById",
                table: "StockMoves",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_LocationDestId",
                table: "StockMoves",
                column: "LocationDestId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_LocationId",
                table: "StockMoves",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_PartnerId",
                table: "StockMoves",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_PickingId",
                table: "StockMoves",
                column: "PickingId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_PickingTypeId",
                table: "StockMoves",
                column: "PickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_ProductId",
                table: "StockMoves",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_WarehouseId",
                table: "StockMoves",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMoves_WriteById",
                table: "StockMoves",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_CompanyId",
                table: "StockPickings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_CreatedById",
                table: "StockPickings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_LocationDestId",
                table: "StockPickings",
                column: "LocationDestId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_LocationId",
                table: "StockPickings",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_PartnerId",
                table: "StockPickings",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_PickingTypeId",
                table: "StockPickings",
                column: "PickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickings_WriteById",
                table: "StockPickings",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_CreatedById",
                table: "StockPickingTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_DefaultLocationDestId",
                table: "StockPickingTypes",
                column: "DefaultLocationDestId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_DefaultLocationSrcId",
                table: "StockPickingTypes",
                column: "DefaultLocationSrcId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_IRSequenceId",
                table: "StockPickingTypes",
                column: "IRSequenceId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_ReturnPickingTypeId",
                table: "StockPickingTypes",
                column: "ReturnPickingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_WarehouseId",
                table: "StockPickingTypes",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPickingTypes_WriteById",
                table: "StockPickingTypes",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuantMoveRel_QuantId",
                table: "StockQuantMoveRel",
                column: "QuantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_CompanyId",
                table: "StockQuants",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_CreatedById",
                table: "StockQuants",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_LocationId",
                table: "StockQuants",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_NegativeMoveId",
                table: "StockQuants",
                column: "NegativeMoveId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_ProductId",
                table: "StockQuants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_PropagatedFromId",
                table: "StockQuants",
                column: "PropagatedFromId");

            migrationBuilder.CreateIndex(
                name: "IX_StockQuants_WriteById",
                table: "StockQuants",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_CompanyId",
                table: "StockWarehouses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_CreatedById",
                table: "StockWarehouses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_InTypeId",
                table: "StockWarehouses",
                column: "InTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_LocationId",
                table: "StockWarehouses",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_OutTypeId",
                table: "StockWarehouses",
                column: "OutTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_PartnerId",
                table: "StockWarehouses",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_ViewLocationId",
                table: "StockWarehouses",
                column: "ViewLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockWarehouses_WriteById",
                table: "StockWarehouses",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_StockPickings_PickingId",
                table: "StockMoves",
                column: "PickingId",
                principalTable: "StockPickings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_StockPickingTypes_PickingTypeId",
                table: "StockMoves",
                column: "PickingTypeId",
                principalTable: "StockPickingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMoves_StockWarehouses_WarehouseId",
                table: "StockMoves",
                column: "WarehouseId",
                principalTable: "StockWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockPickings_StockPickingTypes_PickingTypeId",
                table: "StockPickings",
                column: "PickingTypeId",
                principalTable: "StockPickingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockPickingTypes_StockWarehouses_WarehouseId",
                table: "StockPickingTypes",
                column: "WarehouseId",
                principalTable: "StockWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockPickingTypes_StockLocations_DefaultLocationDestId",
                table: "StockPickingTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_StockPickingTypes_StockLocations_DefaultLocationSrcId",
                table: "StockPickingTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_StockLocations_LocationId",
                table: "StockWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_StockLocations_ViewLocationId",
                table: "StockWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_StockPickingTypes_InTypeId",
                table: "StockWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_StockWarehouses_StockPickingTypes_OutTypeId",
                table: "StockWarehouses");

            migrationBuilder.DropTable(
                name: "StockQuantMoveRel");

            migrationBuilder.DropTable(
                name: "StockQuants");

            migrationBuilder.DropTable(
                name: "StockMoves");

            migrationBuilder.DropTable(
                name: "StockPickings");

            migrationBuilder.DropTable(
                name: "StockLocations");

            migrationBuilder.DropTable(
                name: "StockPickingTypes");

            migrationBuilder.DropTable(
                name: "StockWarehouses");
        }
    }
}
