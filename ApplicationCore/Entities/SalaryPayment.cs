using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SalaryPayment : BaseEntity
    {
        public SalaryPayment()
        {
            State = "waiting";
        }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Sổ nhật ký: tiền mặt, ngân hàng
        /// </summary>
        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        /// <summary>
        /// waiting: chờ xác nhận
        /// done: xác nhận
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

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// nhân viên
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Không sử dụng nữa
        /// </summary>
        public Guid? MoveId { get; set; }
        public AccountMove Move { get; set; }

        /// <summary>
        /// Số tiền bằng chữ
        /// </summary>
        [NotMapped]
        public string AmountText
        {
            get
            {

                return AmountToText.amount_to_text(Amount);
            }
            set { }
        }
    }
}
