using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SalaryPaymentVm
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// draft: Nháp
        /// posted: Đã vào sổ
        /// </summary>
        public string State { get; set; }

        /// <summary>
        ///  ADVANCE/năm/sequence
        ///  SALARY/năm/sequence
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// advance : tạm ứng
        /// salary: chi lương
        /// </summary>
        public string Type { get; set; }

        public Guid? EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }

        public Guid? JournalId { get; set; }
        public AccountJournalSimple journal {get;set;}

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }
    }
}
