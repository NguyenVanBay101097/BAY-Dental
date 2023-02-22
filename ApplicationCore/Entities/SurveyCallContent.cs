using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SurveyCallContent: BaseEntity
    {
        public SurveyCallContent()
        {
            Date = DateTime.Now;
        }

        /// <summary>
        /// nội dung cuộc gọi
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ngày nào
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// thuộc 1 phiếu phân việc đánh giá
        /// </summary>
        public SurveyAssignment Assignment { get; set; }
        public Guid? AssignmentId { get; set; }
    }
}
