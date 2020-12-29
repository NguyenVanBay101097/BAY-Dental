using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateViewVFundBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("if exists (SELECT * FROM sys.views WHERE name='VFundBooks') DROP VIEW VFundBooks");
            migrationBuilder.Sql("CREATE VIEW VFundBooks AS (" +
            " SELECT tc.Id AS ResId, 'phieu.thu.chi' as ResModel, [Date], tc.[Name], tc.Type, ltc.Name AS Type2, Amount, [State], PayerReceiver AS 'RecipientPayer', jn.Name JournalName ,jn.Id as JournalId, tc.CompanyId " +
            "FROM PhieuThuChis tc INNER JOIN " +
            "AccountJournals jn ON tc.JournalId = jn.id INNER JOIN " +
            "LoaiThuChis ltc ON tc.LoaiThuChiId = ltc.Id " +
            "UNION " +
            " SELECT ap.Id,'account.payment', PaymentDate, ap.Name, PaymentType, PaymentType + '-' + ap.PartnerType, Amount AS 'Số tiền', [State], pn.Name, jn.Name,jn.id,ap.CompanyId " +
            "FROM AccountPayments ap INNER JOIN " +
            "Partners pn ON pn.Id = ap.PartnerId INNER JOIN " +
            "AccountJournals jn ON ap.JournalId = jn.id " +
            "UNION " +
            "SELECT sp.Id,'salary.payment' , [Date], sp.[Name], sp.Type, sp.Type, Amount, [State], em.Name, jn.Name,jn.Id,sp.CompanyId " +
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
