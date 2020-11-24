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

    public class SalaryPaymentPrintVm
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        public Guid? CompanyId { get; set; }
        public CompanyBasic Company { get; set; }


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
            get
            {

                var amountText = StringUtils.ChuyenSo(Amount.ToString()); 
                return amountText;
            }
            set { }
        }

        public string UserName { get; set; }

        public Guid? CreateById { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }
    }

    public class SalaryPaymentBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

    }
}
