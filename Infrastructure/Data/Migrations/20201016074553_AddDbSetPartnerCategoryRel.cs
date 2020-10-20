using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddDbSetPartnerCategoryRel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPartnerCategoryRel_PartnerCategories_CategoryId",
                table: "PartnerPartnerCategoryRel");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPartnerCategoryRel_Partners_PartnerId",
                table: "PartnerPartnerCategoryRel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerPartnerCategoryRel",
                table: "PartnerPartnerCategoryRel");

            migrationBuilder.RenameTable(
                name: "PartnerPartnerCategoryRel",
                newName: "PartnerPartnerCategoryRels");

            migrationBuilder.RenameIndex(
                name: "IX_PartnerPartnerCategoryRel_PartnerId",
                table: "PartnerPartnerCategoryRels",
                newName: "IX_PartnerPartnerCategoryRels_PartnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerPartnerCategoryRels",
                table: "PartnerPartnerCategoryRels",
                columns: new[] { "CategoryId", "PartnerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPartnerCategoryRels_PartnerCategories_CategoryId",
                table: "PartnerPartnerCategoryRels",
                column: "CategoryId",
                principalTable: "PartnerCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPartnerCategoryRels_Partners_PartnerId",
                table: "PartnerPartnerCategoryRels",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPartnerCategoryRels_PartnerCategories_CategoryId",
                table: "PartnerPartnerCategoryRels");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPartnerCategoryRels_Partners_PartnerId",
                table: "PartnerPartnerCategoryRels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerPartnerCategoryRels",
                table: "PartnerPartnerCategoryRels");

            migrationBuilder.RenameTable(
                name: "PartnerPartnerCategoryRels",
                newName: "PartnerPartnerCategoryRel");

            migrationBuilder.RenameIndex(
                name: "IX_PartnerPartnerCategoryRels_PartnerId",
                table: "PartnerPartnerCategoryRel",
                newName: "IX_PartnerPartnerCategoryRel_PartnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerPartnerCategoryRel",
                table: "PartnerPartnerCategoryRel",
                columns: new[] { "CategoryId", "PartnerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPartnerCategoryRel_PartnerCategories_CategoryId",
                table: "PartnerPartnerCategoryRel",
                column: "CategoryId",
                principalTable: "PartnerCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPartnerCategoryRel_Partners_PartnerId",
                table: "PartnerPartnerCategoryRel",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
