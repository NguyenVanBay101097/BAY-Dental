using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Districts_DistrictId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Provinces_ProvinceId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Districts_WardId",
                table: "Partners");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Partners_DistrictId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ProvinceId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_WardId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Partners");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Partners",
                newName: "WardName");

            migrationBuilder.AddColumn<int>(
                name: "BirthDay",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BirthMonth",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BirthYear",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicalHistory",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardCode",
                table: "Partners",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartnerCategories",
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
                    table.PrimaryKey("PK_PartnerCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerCategories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerCategories_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartnerPartnerCategoryRel",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerPartnerCategoryRel", x => new { x.CategoryId, x.PartnerId });
                    table.ForeignKey(
                        name: "FK_PartnerPartnerCategoryRel_PartnerCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PartnerCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerPartnerCategoryRel_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerCategories_CreatedById",
                table: "PartnerCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerCategories_WriteById",
                table: "PartnerCategories",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPartnerCategoryRel_PartnerId",
                table: "PartnerPartnerCategoryRel",
                column: "PartnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerPartnerCategoryRel");

            migrationBuilder.DropTable(
                name: "PartnerCategories");

            migrationBuilder.DropColumn(
                name: "BirthDay",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "BirthMonth",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "BirthYear",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "CityCode",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "MedicalHistory",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "Partners");

            migrationBuilder.RenameColumn(
                name: "WardName",
                table: "Partners",
                newName: "Type");

            migrationBuilder.AddColumn<Guid>(
                name: "DistrictId",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProvinceId",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WardId",
                table: "Partners",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    WriteById = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provinces_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Provinces_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ProvinceId = table.Column<Guid>(nullable: false),
                    WriteById = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Districts_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DistrictId = table.Column<Guid>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    WriteById = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wards_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wards_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partners_DistrictId",
                table: "Partners",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ProvinceId",
                table: "Partners",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_WardId",
                table: "Partners",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CreatedById",
                table: "Districts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_WriteById",
                table: "Districts",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_CreatedById",
                table: "Provinces",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_WriteById",
                table: "Provinces",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_CreatedById",
                table: "Wards",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictId",
                table: "Wards",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_WriteById",
                table: "Wards",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Districts_DistrictId",
                table: "Partners",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Provinces_ProvinceId",
                table: "Partners",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Districts_WardId",
                table: "Partners",
                column: "WardId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
