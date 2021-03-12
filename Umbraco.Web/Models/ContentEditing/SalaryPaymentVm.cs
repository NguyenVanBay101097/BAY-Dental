using ApplicationCore.Utilities;
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

        public Guid? CompanyId { get; set; }

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
        public AccountJournalSimple Journal {get;set;}

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }
    }

    public class SalaryPaymentPrintVm
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        public Guid CompanyId { get; set; }
        public CompanyPrintVM Company { get; set; }


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
        public AccountJournalSimple journal { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        public string AmountString
        {
            get; set;
        }

        public string UserName { get; set; }

        public string CreatedById { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }
    }

    public class SalaryPaymentIds
    {
        public ICollection<Guid> Ids { get; set; } = new List<Guid>();
    }

    public class SalaryPaymentSalary
    {
        public ICollection<MultiSalaryPaymentVm> MultiSalaryPayments { get; set; } = new List<MultiSalaryPaymentVm>();
    }

    public class SalaryPaymentBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        public string JournalName { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        public string EmployeeName { get; set; }
    }
}
