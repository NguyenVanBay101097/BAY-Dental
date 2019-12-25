using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F113_FixSaleReportQuotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='sale_report') DROP VIEW sale_report");
            migrationBuilder.Sql("CREATE VIEW sale_report AS ( " +
                "SELECT l.ProductId as ProductId, " +
                "p.UOMId as ProductUOMId, " +
                "SUM(l.ProductUOMQty) as ProductUOMQty, " +
                "SUM(l.QtyInvoiced) as QtyInvoiced, " +
                "SUM(l.QtyToInvoice) as QtyToInvoice, " +
                "SUM(l.PriceTotal) as PriceTotal, " +
                "SUM(l.PriceSubTotal) as PriceSubTotal, " +
                "COUNT(*) as Nbr, " +
                "s.Name as Name, " +
                "s.DateOrder as Date, " +
                "s.State as State, " +
                "s.IsQuotation as IsQuotation, " +
                "s.PartnerId as PartnerId, " +
                "s.UserId as UserId, " +
                "s.CompanyId as CompanyId, " +
                "p.CategId as CategId " +
                "FROM SaleOrderLines l " +
                "JOIN SaleOrders s on l.OrderId = s.Id " +
                "JOIN Partners partner on s.PartnerId = partner.Id " +
                "LEFT JOIN Products p on l.ProductId = p.Id " +
                "GROUP BY l.ProductId, " +
                "l.OrderId, " +
                "p.UOMId, " +
                "p.CategId, " +
                "s.Name, " +
                "s.IsQuotation, " +
                "s.DateOrder, " +
                "s.PartnerId, " +
                "s.UserId, " +
                "s.State, " +
                "s.CompanyId)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW sale_report");
        }
    }
}
