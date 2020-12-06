using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Đợt lương
    /// </summary>
    public class HrPayslipRun: BaseEntity
    {
        public HrPayslipRun()
        {
            State = "confirm";
        }

        public string Name { get; set; }
        /// <summary>
        /// confirm(chờ xác nhận), done(hoàn thành, đã xác nhận)
        /// </summary>
        public string State { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<HrPayslip> Slips { get; set; } = new List<HrPayslip>();
        /// <summary>
        /// lương tháng nào năm nào?
        /// </summary>
        public DateTime? Date { get; set; }
        public Guid? MoveId { get; set; }
        public AccountMove Move { get; set; }
    }
}
