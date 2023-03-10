using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Chi nhánh
    /// </summary>
    public class Company: BaseEntity
    {
        public Company()
        {
            Active = true;
        }

        public string Name { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime? PeriodLockDate { get; set; }

        /// <summary>
        /// Tài khoản doanh thu
        /// </summary>
        public Guid? AccountIncomeId { get; set; }
        public AccountAccount AccountIncome { get; set; }

        /// <summary>
        /// Tài khoản chi phí
        /// </summary>
        public Guid? AccountExpenseId { get; set; }
        public AccountAccount AccountExpense { get; set; }

        public string Logo { get; set; }

        /// <summary>
        /// Trạng thái chi nhánh
        /// </summary>
        public bool Active { get; set; }

        [NotMapped]
        public string Address 
        {
            get
            {
                return Partner != null ? Partner.GetAddress() : "";
            }
        }

        public string ReportHeader { get; set; }

        public string ReportFooter { get; set; }
    }
}
