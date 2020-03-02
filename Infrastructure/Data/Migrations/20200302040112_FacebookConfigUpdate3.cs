using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FacebookConfigUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookPageUsers");

            migrationBuilder.CreateTable(
                name: "FacebookUserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PsId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    PageId = table.Column<string>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookUserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookUserProfiles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookUserProfiles_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookUserProfiles_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacebookConversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    FacebookObjectId = table.Column<string>(nullable: true),
                    FacebookPageId = table.Column<string>(nullable: true),
                    Snippet = table.Column<string>(nullable: true),
                    MessageCount = table.Column<int>(nullable: false),
                    UnreadCount = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookConversations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookConversations_FacebookUserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "FacebookUserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookConversations_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConversations_CreatedById",
                table: "FacebookConversations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConversations_UserId",
                table: "FacebookConversations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConversations_WriteById",
                table: "FacebookConversations",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookUserProfiles_CreatedById",
                table: "FacebookUserProfiles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookUserProfiles_PartnerId",
                table: "FacebookUserProfiles",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookUserProfiles_WriteById",
                table: "FacebookUserProfiles",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookConversations");

            migrationBuilder.DropTable(
                name: "FacebookUserProfiles");

            migrationBuilder.CreateTable(
                name: "FacebookPageUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfigPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Psid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WriteById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookPageUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookPageUsers_FacebookConfigPages_ConfigPageId",
                        column: x => x.ConfigPageId,
                        principalTable: "FacebookConfigPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookPageUsers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookPageUsers_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookPageUsers_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPageUsers_ConfigPageId",
                table: "FacebookPageUsers",
                column: "ConfigPageId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPageUsers_CreatedById",
                table: "FacebookPageUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPageUsers_PartnerId",
                table: "FacebookPageUsers",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPageUsers_WriteById",
                table: "FacebookPageUsers",
                column: "WriteById");
        }
    }
}
