using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateCommissionAgent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgentId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommissionId",
                table: "Agents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_AgentId",
                table: "CommissionSettlements",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_CommissionId",
                table: "Agents",
                column: "CommissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Commissions_CommissionId",
                table: "Agents",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Agents_AgentId",
                table: "CommissionSettlements",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Commissions_CommissionId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Agents_AgentId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_AgentId",
                table: "CommissionSettlements");

            migrationBuilder.DropIndex(
                name: "IX_Agents_CommissionId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "CommissionId",
                table: "Agents");
        }
    }
}
