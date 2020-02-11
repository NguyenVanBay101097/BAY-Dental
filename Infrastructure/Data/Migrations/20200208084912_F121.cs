using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F121 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IRModelFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Model = table.Column<string>(nullable: false),
                    IRModelId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    TType = table.Column<string>(nullable: false),
                    Relation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRModelFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRModelFields_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRModelFields_IRModels_IRModelId",
                        column: x => x.IRModelId,
                        principalTable: "IRModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IRModelFields_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IRProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ValueFloat = table.Column<double>(nullable: true),
                    ValueInteger = table.Column<int>(nullable: true),
                    ValueText = table.Column<string>(nullable: true),
                    ValueBinary = table.Column<byte[]>(nullable: true),
                    ValueReference = table.Column<string>(nullable: true),
                    ValueDateTime = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    FieldId = table.Column<Guid>(nullable: false),
                    ResId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRProperties_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRProperties_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRProperties_IRModelFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "IRModelFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IRProperties_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IRModelFields_CreatedById",
                table: "IRModelFields",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelFields_IRModelId",
                table: "IRModelFields",
                column: "IRModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelFields_WriteById",
                table: "IRModelFields",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRProperties_CompanyId",
                table: "IRProperties",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IRProperties_CreatedById",
                table: "IRProperties",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRProperties_FieldId",
                table: "IRProperties",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_IRProperties_WriteById",
                table: "IRProperties",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IRProperties");

            migrationBuilder.DropTable(
                name: "IRModelFields");
        }
    }
}
