using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddFacebookConnectTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Partners_PartnerMapPSIDFacebookPages_PartnerMapPSIDFacebookPageId",
            //    table: "Partners");

            //migrationBuilder.DropIndex(
            //    name: "IX_Partners_PartnerMapPSIDFacebookPageId",
            //    table: "Partners");

            //migrationBuilder.DropColumn(
            //    name: "PartnerMapPSIDFacebookPageId",
            //    table: "Partners");

            migrationBuilder.CreateTable(
                name: "FacebookConnects",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    FbUserId = table.Column<string>(nullable: true),
                    FbUserName = table.Column<string>(nullable: true),
                    FbUserAccessToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookConnects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookConnects_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookConnects_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacebookConnectPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PageId = table.Column<string>(nullable: true),
                    PageName = table.Column<string>(nullable: true),
                    PageAccessToken = table.Column<string>(nullable: true),
                    ConnectId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookConnectPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookConnectPages_FacebookConnects_ConnectId",
                        column: x => x.ConnectId,
                        principalTable: "FacebookConnects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacebookConnectPages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookConnectPages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConnectPages_ConnectId",
                table: "FacebookConnectPages",
                column: "ConnectId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConnectPages_CreatedById",
                table: "FacebookConnectPages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConnectPages_WriteById",
                table: "FacebookConnectPages",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConnects_CreatedById",
                table: "FacebookConnects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConnects_WriteById",
                table: "FacebookConnects",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookConnectPages");

            migrationBuilder.DropTable(
                name: "FacebookConnects");

            //migrationBuilder.AddColumn<Guid>(
            //    name: "PartnerMapPSIDFacebookPageId",
            //    table: "Partners",
            //    type: "uniqueidentifier",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Partners_PartnerMapPSIDFacebookPageId",
            //    table: "Partners",
            //    column: "PartnerMapPSIDFacebookPageId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Partners_PartnerMapPSIDFacebookPages_PartnerMapPSIDFacebookPageId",
            //    table: "Partners",
            //    column: "PartnerMapPSIDFacebookPageId",
            //    principalTable: "PartnerMapPSIDFacebookPages",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
