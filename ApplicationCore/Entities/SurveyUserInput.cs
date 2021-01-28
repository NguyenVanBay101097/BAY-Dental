using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Bảng làm khảo sát
    /// </summary>
    public class SurveyUserInput : BaseEntity
    {
        /// <summary>
        /// Số điểm đạt được, compute
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// Số điểm tối đa, compute
        /// </summary>
        public decimal? MaxScore { get; set; }

        /// <summary>
        /// thuộc 1 phân việc khảo sát
        /// </summary>
        public Guid SurveyAssignmentId { get; set; }
        public SurveyAssignment SurveyAssignment{ get; set; }
        /// <summary>
        /// danh sách kết quả câu hỏi kèm câu trả lời của khách hàng
        /// </summary>
        public ICollection<SurveyUserInputLine> Lines { get; set; } = new List<SurveyUserInputLine>();

        public ICollection<SurveyCallContent> CallContents { get; set; } = new List<SurveyCallContent>();
    }
}
