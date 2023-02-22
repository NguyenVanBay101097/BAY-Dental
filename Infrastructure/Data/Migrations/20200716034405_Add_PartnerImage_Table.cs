using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Add_PartnerImage_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartnerImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    DotkhamId = table.Column<Guid>(nullable: true),
                    UploadId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerImages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerImages_DotKhams_DotkhamId",
                        column: x => x.DotkhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerImages_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerImages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerImages_CreatedById",
                table: "PartnerImages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerImages_DotkhamId",
                table: "PartnerImages",
                column: "DotkhamId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerImages_PartnerId",
                table: "PartnerImages",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerImages_WriteById",
                table: "PartnerImages",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerImages");
        }
    }
}
