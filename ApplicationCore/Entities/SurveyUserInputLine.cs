using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SurveyUserInputLine: BaseEntity
    {
        /// <summary>
        /// số điểm của câu trả lời
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// kết quả câu trả lời text
        /// </summary>
        public string ValueText { get; set; }

        /// <summary>
        /// thuộc 1 phiếu khảo sát
        /// </summary>
        public Guid UserInputId { get; set; }
        public SurveyUserInput UserInput  { get; set; }

        /// <summary>
        /// nếu là radio thì dựa vào Id để match câu trả lời, delete cascade
        /// </summary>
        public Guid? AnswerId { get; set; }
        public SurveyAnswer Answer { get; set; }
        /// <summary>
        /// cho 1 câu hỏi
        /// </summary>
        public Guid? QuestionId { get; set; }
        public SurveyQuestion Question { get; set; }
    }
}
