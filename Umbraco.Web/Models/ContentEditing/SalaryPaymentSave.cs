using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SalaryPaymentSave
    {
        public DateTime Date { get; set; }

        /// <summary>
        /// Sổ nhật ký: tiền mặt, ngân hàng
        /// </summary>
        public Guid JournalId { get; set; }

        /// <summary>
        /// nhân viên
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// advance : tạm ứng
        /// salary: chi lương
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }
    }

    public class SalaryPaymentDefaultGetModel
    {
        public Guid? PayslipRunId { get; set; }
        public IEnumerable<Guid> PayslipIds { get; set; }
    }
}
