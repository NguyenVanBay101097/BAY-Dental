using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateViewVFundBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='VFundBooks') DROP VIEW VFundBooks");
            migrationBuilder.Sql("CREATE VIEW VFundBooks AS ( " +
            "SELECT tc.Id AS ResId, 'phieu.thu.chi' as ResModel, [Date], tc.[Name], " +
            "case when tc.Type = 'thu' then 'inbound' else 'outbound' end as Type, " +
            "ltc.Name AS Type2, Amount, case when tc.Type = 'thu' then Amount else -Amount end as AmountSigned, [State], PayerReceiver AS 'RecipientPayer', jn.Id as JournalId, tc.CompanyId " +
            "FROM PhieuThuChis tc INNER JOIN " +
            "AccountJournals jn ON tc.JournalId = jn.id INNER JOIN " +
            "LoaiThuChis ltc ON tc.LoaiThuChiId = ltc.Id " +
            "UNION " +
            "SELECT ap.Id,'account.payment', PaymentDate, ap.Name, PaymentType, " +
            "case when ap.PaymentType='inbound' AND ap.PartnerType = 'customer' then N'Khách hàng thanh toán' " +
            "when ap.PaymentType='inbound' AND ap.PartnerType = 'supplier' then N'NCC hoàn tiền' " +
            "when ap.PaymentType='outbound' AND ap.PartnerType = 'customer' then N'Hoàn tiền khách hàng' " +
            "when ap.PaymentType='outbound' AND ap.PartnerType = 'supplier' then N'Thanh toán NCC' end, " +
            "Amount, case when ap.PaymentType = 'inbound' then Amount else -Amount end, [State], pn.Name, jn.id,ap.CompanyId " +
            "FROM AccountPayments ap INNER JOIN " +
            "Partners pn ON pn.Id = ap.PartnerId INNER JOIN " +
            "AccountJournals jn ON ap.JournalId = jn.id " +
            "UNION " +
            "SELECT sp.Id,'salary.payment' , [Date], sp.[Name], " +
            "'outbound', " +
            "case when sp.Type = 'salary' then N'Chi lương' when sp.Type = 'advance' then N'Tạm ứng lương' end " +
            ", Amount, -Amount,case when State = 'done' then 'posted' else 'draft' end, em.Name, jn.Id,sp.CompanyId " +
            "FROM SalaryPayments sp INNER JOIN " +
            "Employees em ON em.Id = sp.EmployeeId INNER JOIN " +
            "AccountJournals jn ON sp.JournalId = jn.id)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW VFundBooks");
        }
    }
}
