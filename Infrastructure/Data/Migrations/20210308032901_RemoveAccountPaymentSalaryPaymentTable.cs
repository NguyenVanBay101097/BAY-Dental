﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class RemoveAccountPaymentSalaryPaymentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslips_AccountPayments_AccountPaymentId",
                table: "HrPayslips");

            migrationBuilder.DropIndex(
                name: "IX_HrPayslips_AccountPaymentId",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "AccountPaymentId",
                table: "HrPayslips");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountPaymentId",
                table: "HrPayslips",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_AccountPaymentId",
                table: "HrPayslips",
                column: "AccountPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslips_AccountPayments_AccountPaymentId",
                table: "HrPayslips",
                column: "AccountPaymentId",
                principalTable: "AccountPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
