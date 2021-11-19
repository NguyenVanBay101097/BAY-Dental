using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddViewPartnerOldNewInSaleOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='partner_old_new_in_sale_order') DROP VIEW partner_old_new_in_sale_order");
            migrationBuilder.Sql("CREATE VIEW partner_old_new_in_sale_order AS " +
                    "( select s.DateOrder, s.Id as 'SaleOrderId' , s.CompanyId, s.PartnerId, p.Name, " +
                "CASE WHEN NOT EXISTS(select 1 from SaleOrders s2 where s2.DateOrder < s.DateOrder and s2.PartnerId = s.PartnerId) " +
                "THEN 1 ELSE 0 END AS IsNew, s.TotalPaid " +
                "from SaleOrders s left join Partners p on s.PartnerId = p.Id)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW partner_old_new_in_sale_order");
        }
    }
}
