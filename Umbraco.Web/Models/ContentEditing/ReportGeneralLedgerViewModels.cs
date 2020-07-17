using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ReportGeneralLedgerValues
    {
        public IEnumerable<ReportGeneralLedgerAccountRes> Accounts { get; set; }
    }

    public class ReportGeneralLedgerAccountRes
    {
        public ReportGeneralLedgerAccountRes()
        {
            MoveLines = new List<ReportGeneralLedgerMoveLine>();
        }

        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public decimal InitialBalance { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal Balance { get; set; }

        public IEnumerable<ReportGeneralLedgerMoveLine> MoveLines { get; set; }
    }

    public class ReportGeneralLedgerMoveLine
    {
        public Guid AccountId { get; set; }

        public Guid? PartnerId { get; set; }

        public DateTime? Date { get; set; }

        public string PartnerName { get; set; }

        public string PartnerRef { get; set; }

        public string Name { get; set; }

        public string AccountInternalType { get; set; }

        public decimal InitialBalance { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal Balance { get; set; }

        public string MoveName { get; set; }

        public string Ref { get; set; }

        public string JournalCode { get; set; }
    }

    public class ReportCashBankGeneralLedger
    {
        /// <summary>
        /// cash_bank : tất cả
        /// bank : tài khoản ngân hàng
        /// cash : tiền mặt
        /// </summary>
        public string ResultSelection { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }
}
