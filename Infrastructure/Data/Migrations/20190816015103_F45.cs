using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F45 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='account_invoice_report') DROP VIEW account_invoice_report");
            migrationBuilder.Sql("CREATE VIEW account_invoice_report AS ( " +
                "SELECT ail.Id AS id, " +
                    "ai.DateInvoice AS date, " +
                    "ai.Number as number, " +
                    "ail.ProductId as product_id, ai.PartnerId as partner_id, " +
                    "ai.JournalId as journal_id, ai.UserId as user_id, ai.CompanyId as company_id, " +
                    "1 AS nbr, " +
                    "ai.id AS invoice_id, ai.Type as type, ai.State as state, pr.CategId as categ_id, ai.AccountId as account_id, ail.AccountId AS account_line_id, " +
                    "SUM((invoice_type.sign_qty * ail.quantity)) AS product_qty, " +
                    "SUM(ail.PriceSubTotalSigned * invoice_type.sign) AS price_total, " +
                     "SUM(ail.PriceSubTotal * invoice_type.sign_qty) AS amount_total, " +
                      "ai.ResidualSigned / (SELECT count(*) FROM AccountInvoiceLines l where InvoiceId = ai.id) * " +
                    "count(*) * invoice_type.sign AS residual, " +
                      "ai.DiscountAmount / (SELECT count(*) FROM AccountInvoiceLines l where InvoiceId = ai.id) * " +
                    "count(*) * invoice_type.sign AS discount_amount " +
                 "FROM AccountInvoiceLines ail " +
                "JOIN AccountInvoices ai ON ai.Id = ail.InvoiceId " +
                "JOIN Partners partner_ai ON ai.PartnerId = partner_ai.Id " +
                "LEFT JOIN Products pr ON pr.Id = ail.ProductId " +
                "JOIN( " +
                "SELECT id, (CASE " +
                         "WHEN ai.Type IN('in_refund', 'in_invoice') " +
                            "THEN - 1 " +
                            "ELSE 1 " +
                        "END) AS sign,(CASE " +
                         "WHEN ai.Type IN('out_refund', 'in_invoice') " +
                            "THEN - 1 " +
                            "ELSE 1 " +
                        "END) AS sign_qty " +
                    "FROM AccountInvoices ai) AS invoice_type ON invoice_type.id = ai.Id " +
                    "WHERE ail.AccountId IS NOT NULL " +
                    "GROUP BY ail.Id, ail.ProductId, ai.Number, ai.DateInvoice, ai.Id, " +
                    "ai.PartnerId, ai.JournalId, " +
                    "ai.UserId, ai.CompanyId, ai.id, ai.type, invoice_type.sign, ai.state, pr.CategId, " +
                    "ai.AccountId, ail.AccountId, ai.ResidualSigned, ai.DiscountAmount, " +
                    "ai.AmountTotalSigned)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW account_invoice_report");
        }
    }
}
