using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SalaryPaymentDisplay
    {
        public SalaryPaymentDisplay()
        {
            Date = DateTime.Today;
            State = "waiting";
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

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
        public AccountJournalSimple Journal { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }
    }

    public class PayslipCreateSalaryPaymentDisplay
    {
        public PayslipCreateSalaryPaymentDisplay()
        {
            Date = DateTime.Today;
        }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        public EmployeeSimple Employee { get; set; }

        public AccountJournalSimple Journal { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        public Guid PayslipId { get; set; }
    }

    public class PayslipCreateSalaryPaymentSave
    {
        public PayslipCreateSalaryPaymentSave()
        {
            Date = DateTime.Today;
        }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid JournalId { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        public Guid PayslipId { get; set; }
    }
}
