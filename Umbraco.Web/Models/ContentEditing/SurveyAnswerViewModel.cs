using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   public class SurveyAnswerBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class SurveyAnswerDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid QuestionId { get; set; }
        public decimal? Score { get; set; }
        public int? Sequence { get; set; }
    }

    public class SurveyAnswerSave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? Score { get; set; }
    }
}
