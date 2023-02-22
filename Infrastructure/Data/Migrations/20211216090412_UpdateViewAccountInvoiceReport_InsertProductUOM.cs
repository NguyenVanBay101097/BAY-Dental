﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateViewAccountInvoiceReport_InsertProductUOM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='account_invoice_report') DROP VIEW account_invoice_report");
			migrationBuilder.Sql(@"
CREATE VIEW account_invoice_report
AS (
					SELECT aml.Id AS id,
                    am.Date AS InvoiceDate,
                    am.InvoiceOrigin as InvoiceOrigin,
                    aml.ProductId as ProductId,
					aml.PartnerId as PartnerId,
                    aml.JournalId as JournalId,
					am.InvoiceUserId as InvoiceUserId,
					aml.CompanyId as CompanyId,
					aml.EmployeeId,
					aml.AssistantId,
                    am.Type as Type,
					aml.AccountInternalType as AccountInternalType,
					am.State as State,
					aml.AccountId as AccountId,
					aml.ProductUoMId as ProductUoMId,
					uom.Name as UoMName,
					SUM ((aml.Quantity)) AS Quantity,
					-SUM(aml.Balance) AS PriceSubTotal

				 FROM AccountMoveLines aml
                JOIN AccountMoves am ON am.Id = aml.MoveId
				JOIN UoMs uom ON aml.ProductUoMId = uom.Id
				JOIN (
				SELECT id,(CASE
                         WHEN am.Type IN ('in_refund', 'in_invoice')
                            THEN -1
                            ELSE 1
                        END) AS sign,(CASE
                         WHEN am.Type IN ('out_refund', 'in_invoice')
                            THEN -1
                            ELSE 1
                        END) AS sign_qty
                    FROM AccountMoves am) AS invoice_type ON invoice_type.id = am.Id

					WHERE aml.AccountId IS NOT NULL 
					and am.Type in ('out_invoice', 'out_refund', 'in_invoice', 'in_refund', 'out_receipt', 'in_receipt')

					GROUP BY aml.Id ,
					 am.Date,
					 am.InvoiceOrigin,
					 aml.ProductId ,
					aml.PartnerId  ,
					 aml.JournalId ,
					am.InvoiceUserId ,
					aml.CompanyId ,
					am.Type,
					am.State ,
					aml.AccountId,
					aml.EmployeeId,
					aml.AssistantId,
					aml.AccountInternalType,
					aml.ProductUoMId,
					uom.Id,
					uom.Name
					)
");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
 DROP VIEW account_invoice_report 
");
		}
    }
}
