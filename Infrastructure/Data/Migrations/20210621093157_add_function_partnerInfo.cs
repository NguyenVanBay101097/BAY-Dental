using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class add_function_partnerInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF exists (select * from sys.objects where   object_id = OBJECT_ID(N'[dbo].[fn_PartnerInfoList]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP function fn_PartnerInfoList
");
            migrationBuilder.Sql(@"
create function [dbo].[fn_PartnerInfoList] (
@CompanyId nvarchar(255)
)

returns TABLE

as
return 

--begin

--  DECLARE @offset INT
--  DECLARE @newsize INT
--  DECLARE @sql NVARCHAR(MAX)
--        SET @offset = (@pageNumer-1)*@pageSize
--        SET @newsize = @pageSize

select  pn.id, pn.Avatar, pn.DateCreated,pn.Ref, pn.name, pn.NameNoSign, pn.DisplayName ,pn.phone, pn.Email,pn.BirthDay,pn.BirthMonth,pn.BirthYear,pn.CompanyId,
case when pn.OrderStates like N'%sale%' then 'sale' when pn.OrderStates like N'%sale%' then 'sale'
when REPLACE(REPLACE(pn.OrderStates, 'done' ,''), ',', '') = '' then 'done' else 'draft' 
end as OrderState,

(select sum(AmountTotal - TotalPaid) from SaleOrders where PartnerId = pn.Id and State in ('sale','done') and CompanyId = @CompanyId) as OrderResidual,

(select SUM(Balance) from AccountMoveLines aml inner join AccountAccounts acc on acc.Id = aml.AccountId 
where aml.PartnerId = pn.Id  and acc.Code ='CNKH' and aml.CompanyId = @CompanyId) as TotalDebit

--,(select Id from MemberLevels 
--where id in (select  SUBSTRING(ValueReference,CHARINDEX(',',ValueReference,0)+1,LEN(ValueReference)) from IRProperties
--where ResId = 'res.partner,'+CONVERT(nvarchar(max),pn.Id) and Name = 'member_level' and CompanyId = @CompanyId
--)) as MemberLevelId
,(select cast(SUBSTRING(ValueReference,CHARINDEX(',',ValueReference,0)+1,LEN(ValueReference)) as UNIQUEIDENTIFIER) from IRProperties
where ResId = 'res.partner,'+CONVERT(nvarchar(max),pn.Id) and Name = 'member_level' and CompanyId = @CompanyId) as MemberLevelId

,stuff((select ','+Convert(nvarchar(max),pnr.CategoryId) 
from PartnerPartnerCategoryRel pnr
where pn.Id = pnr.PartnerId for xml path (''), type).value('.','nvarchar(max)') ,1,1,'') as PartnerCategIds

from
(
select *,
stuff((select ','+state from saleorders where pn.id = PartnerId for xml path (''), type).value('.','nvarchar(max)') ,1,1,'') as OrderStates
from Partners pn
where pn.Customer = 1 and pn.CompanyId = @CompanyId
)
as pn
--order by pn.DateCreated
--OFFSET (@pageNumer-1)*@pageSize rows fetch next @pageSize rows only
--end
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP function fn_PartnerInfoList");
        }
    }
}
