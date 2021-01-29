using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SurveyUserInputLineBasic
    {
        public Guid Id { get; set; }

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
        public SurveyUserInputBasic UserInput { get; set; }

        /// <summary>
        /// nếu là radio thì dựa vào Id để match câu trả lời, delete cascade
        /// </summary>
        public Guid? AnswerId { get; set; }
        public SurveyAnswerBasic Answer { get; set; }
        /// <summary>
        /// cho 1 câu hỏi
        /// </summary>
        public Guid? QuestionId { get; set; }
        public SurveyQuestionBasic Question { get; set; }
    }

    public class SurveyUserInputLineSave
    {
        public decimal? Score { get; set; }

        /// <summary>
        /// kết quả câu trả lời text
        /// </summary>
        public string ValueText { get; set; }

        /// <summary>
        /// thuộc 1 phiếu khảo sát
        /// </summary>
        public Guid UserInputId { get; set; }

        /// <summary>
        /// nếu là radio thì dựa vào Id để match câu trả lời, delete cascade
        /// </summary>
        public Guid? AnswerId { get; set; }

        /// <summary>
        /// cho 1 câu hỏi
        /// </summary>
        public Guid? QuestionId { get; set; }
    }

    public class SurveyUserInputLineDisplay
    {
        public Guid Id { get; set; }

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
        public SurveyUserInputDisplay UserInput { get; set; }

        /// <summary>
        /// nếu là radio thì dựa vào Id để match câu trả lời, delete cascade
        /// </summary>
        public Guid? AnswerId { get; set; }
        public SurveyAnswerDisplay Answer { get; set; }
        /// <summary>
        /// cho 1 câu hỏi
        /// </summary>
        public Guid? QuestionId { get; set; }
        public SurveyQuestionDisplay Question { get; set; }
    }
}
