using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddViewPartnerOldNewReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='partner_old_new_report') DROP VIEW partner_old_new_report");
            migrationBuilder.Sql("CREATE VIEW partner_old_new_report AS ( " +
            "select od.DateOrder as [Date], pn.Name  as 'PartnerName',pn.Id as 'PartnerId',od.Id as 'OrderId', od.name as 'OrderName', " +
            "(select count(id) from saleOrderlines where OrderPartnerId = pn.Id and OrderId = od.id) as 'CountLine', " +
            "(CASE When " +
            "(select count(*) from saleOrderlines odl inner join saleorders sod on odl.OrderId = sod.Id " +
            "where odl.OrderPartnerId = ol.OrderPartnerId ANd sod.DateOrder < od.DateOrder) >= 1 then 'KHC' else 'KHM' end) as 'Type', ol.CompanyId " +
            "from SaleOrderLines ol " +
            "inner join SaleOrders od on ol.OrderId = od.id " +
            "inner join partners pn on pn.id = ol.OrderPartnerId " +
            "group by ol.OrderPartnerId, od.DateOrder, od.id, od.Name,pn.Id,pn.Name, ol.CompanyId)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW partner_old_new_report");
        }
    }
}
