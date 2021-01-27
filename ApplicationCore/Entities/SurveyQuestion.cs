using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SurveyQuestion : BaseEntity
    {
        /// <summary>
        /// Tên câu hỏi
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// loại câu hỏi: loại radio hoặc loại text
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// list câu trả lời
        /// </summary>
        public ICollection<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
