using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SurveyCallContent: BaseEntity
    {
        /// <summary>
        /// nội dung cuộc gọi
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// số thứ tự
        /// </summary>
        public int? Sequence { get; set; }
        /// <summary>
        /// thuộc 1 phiếu phân việc đánh giá
        /// </summary>
        public SurveyUserInput UserInput { get; set; }
        public Guid? UserInputId { get; set; }
    }
}
