using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MultiSalaryPaymentVm
    {

        public DateTime Date { get; set; }

        /// <summary>
        /// Sổ nhật ký: tiền mặt, ngân hàng
        /// </summary>
        public Guid? JournalId { get; set; }
        // public AccountJournalSimple Journal { get; set; }

        /// <summary>
        /// nhân viên
        /// </summary>
        public Guid? EmployeeId { get; set; }
        // public EmployeeSimple Employee { get; set; }


        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal? Amount { get; set; }


        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        public Guid? HrPayslipId { get; set; }

    }
}
