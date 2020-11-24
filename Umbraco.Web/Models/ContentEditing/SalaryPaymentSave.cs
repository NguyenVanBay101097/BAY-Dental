using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SalaryPaymentSave
    {
        public SalaryPaymentSave(){
            State = "waitting";
        }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }

        public string Name { get; set; }

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
        /// waitting: chờ xác nhận
        /// done: xác nhận
        /// </summary>
        public string State { get; set; }

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
