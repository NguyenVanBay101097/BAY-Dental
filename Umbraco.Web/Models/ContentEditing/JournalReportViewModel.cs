using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class JournalReport
    {
        public decimal DebitSum { get; set; }

        public decimal CreditSum { get; set; }
        public decimal BalanceSum { get; set; }

        public Guid? JournalId { get; set; }

        public string Name { get; set; }

        public int? Year { get; set; }
        public int? Week { get; set; }
        public int? Quarter { get; set; }

        public DateTime? Date { get; set; }

        public string GroupBy { get; set; }
    }

    public class JournalReportPaged
    {
        public JournalReportPaged()
        {
            GroupBy = "journal";
        }


        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string Search { get; set; }
        public string GroupBy { get; set; }
        /// <summary>
        /// all : tất cả
        /// bank : tài khoản ngân hàng
        /// cash : tiền mặt
        /// </summary>
        public string Filter { get; set; }

    }

    public class JournalReportDetailPaged
    {
        public Guid? JournalId { get; set; }        

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string GroupBy { get; set; }
    }
}
