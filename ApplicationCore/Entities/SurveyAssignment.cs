using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SurveyAssignment: BaseEntity
    {

        public SurveyAssignment()
        {
            Status = "draft";
        }

        /// <summary>
        /// phân việc khảo sát giao cho 1 nhân viên
        /// </summary>
        public Guid EmployeeId { get; set; }
        public Employee employee { get; set; }
        /// <summary>
        /// phân việc khảo sát của 1 phiếu điều trị
        /// </summary>
        public Guid SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }
        /// <summary>
        /// trạng thái: draft(chưa gọi), nocontact(chưa liên hệ được), done(hoàn thành)
        /// </summary>
        public string Status { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
