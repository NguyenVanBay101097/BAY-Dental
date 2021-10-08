using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class SalaryPaymentsPrint
    {
        public IEnumerable<SalaryPaymentPrintTemplate> Salaries { get; set; } = new List<SalaryPaymentPrintTemplate>();
    }

    public class SalaryPaymentPrintTemplate
    {
        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        public CompanyPrintTemplate Company { get; set; }


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

        public EmployeePrintTemplate Employee { get; set; }

        public Guid? JournalId { get; set; }
        public AccountJournalSimplePrintTemplate journal { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        public string AmountText
        {
            get { return AmountToText.amount_to_text(Amount); }
            set { }
        }

        public string UserName { get; set; }

        public string CreatedById { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }
    }

    public class AccountJournalSimplePrintTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
