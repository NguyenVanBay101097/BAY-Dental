using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F41 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IRModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Model = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRModels_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRModels_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResGroups",
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
                    table.PrimaryKey("PK_ResGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResGroups_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResGroups_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IRModelAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    PermRead = table.Column<bool>(nullable: false),
                    PermWrite = table.Column<bool>(nullable: false),
                    PermCreate = table.Column<bool>(nullable: false),
                    PermUnlink = table.Column<bool>(nullable: false),
                    ModelId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRModelAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRModelAccesses_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRModelAccesses_ResGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IRModelAccesses_IRModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "IRModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IRModelAccesses_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResGroupsUsersRels",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResGroupsUsersRels", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ResGroupsUsersRels_ResGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResGroupsUsersRels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IRModelAccesses_CreatedById",
                table: "IRModelAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelAccesses_GroupId",
                table: "IRModelAccesses",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelAccesses_ModelId",
                table: "IRModelAccesses",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IRModelAccesses_WriteById",
                table: "IRModelAccesses",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModels_CreatedById",
                table: "IRModels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRModels_WriteById",
                table: "IRModels",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroups_CreatedById",
                table: "ResGroups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroups_WriteById",
                table: "ResGroups",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroupsUsersRels_UserId",
                table: "ResGroupsUsersRels",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IRModelAccesses");

            migrationBuilder.DropTable(
                name: "ResGroupsUsersRels");

            migrationBuilder.DropTable(
                name: "IRModels");

            migrationBuilder.DropTable(
                name: "ResGroups");
        }
    }
}
