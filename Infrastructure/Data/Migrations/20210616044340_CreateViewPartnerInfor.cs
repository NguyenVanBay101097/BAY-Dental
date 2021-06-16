using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateViewPartnerInfor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.procedures WHERE name='prd_PartnerInfo') DROP VIEW prdPartnerInfo");
            migrationBuilder.Sql("CREATE VIEW vPartnerInfo AS (" +
            "select Id,Name,Ref,Phone,BirthDay,BirthMonth,BirthYear,Residual,TotalDebit, " +
            "case when States is null then 'draft' when States like N'%sale%' then 'sale' else 'done' end as State, " +
            "MemberLevel,PartnerCategIds from ( " +
            "Select pn.id as 'Id', pn.name as 'Name',pn.Ref as 'Ref' , pn.phone as 'Phone', pn.BirthDay as 'BirthDay', pn.BirthMonth as 'BirthMonth', pn.BirthYear as 'BirthYear', " +
            "sum(od.Residual) as 'Residual', " +
            "sum(aml.Balance) as 'TotalDebit'," +
            "stuff((select ','+state from saleorders where pn.id = partnerid for xml path (''), type).value('.','nvarchar(max)') ,1,1,'') as 'States', " +
            "'MemberLevel' = (select ValueFloat from IRProperties where ('res.partner,'+CONVERT(nvarchar(max),pn.Id)) = ResId and Name = 'loyalty_points'), " +
            "stuff((select ','+Convert(nvarchar(max),pnc.Id) from PartnerPartnerCategoryRel pnr inner join PartnerCategories pnc on pnc.Id = pnr.CategoryId where pn.Id = pnr.PartnerId for xml path (''), type).value('.','nvarchar(max)') ,1,1,'') as 'PartnerCategIds' " +
            "from AccountMoveLines aml inner join AccountAccounts acc on (acc.Id = aml.AccountId and acc.Code ='CNKH') " +
            "right join Partners pn on aml.PartnerId = pn.Id " +
            "left join SaleOrders od on od.PartnerId = pn.Id " +
            "where pn.Customer = 1 " +
            "group by pn.id, pn.name,pn.Ref ,pn.phone,pn.BirthDay,pn.BirthMonth,pn.BirthYear) as vPartnerInfo )");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW vPartnerInfo");
        }
    }
}
