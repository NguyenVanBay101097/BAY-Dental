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
        /// trạng thái: draft(chưa gọi), contact(liên hệ ), done(hoàn thành)
        /// </summary>
        public string Status { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        /// <summary>
        /// ngày hoàn thành khảo sát
        /// </summary>
        public DateTime? CompleteDate { get; set; }

        /// <summary>
        /// có 1 phiếu khảo sát
        /// </summary>
        public Guid? UserInputId { get; set; }
        public SurveyUserInput UserInput { get; set; }
        /// <summary>
        /// có 1 list chăm sóc gọi điện
        /// </summary>
        public ICollection<SurveyCallContent> CallContents { get; set; } = new List<SurveyCallContent>();

        /// <summary>
        /// gán khách hàng cho 1 phân quyền
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

    }
}
