using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SurveyAnswer: BaseEntity
    {
        public string Name { get; set; }
        /// <summary>
        /// thuộc câu hỏi nào
        /// </summary>
        public Guid QuestionId { get; set; }
        public SurveyQuestion Question { get; set; }
        /// <summary>
        /// thang điểm
        /// </summary>
        public decimal? Score { get; set; }
        /// <summary>
        /// thứ tự
        /// </summary>
        public int? Sequence { get; set; }
    }
}
