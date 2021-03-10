using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class SurveyQuestionPaged {
        public SurveyQuestionPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public string Type { get; set; }
        public bool? Active { get; set; }

    }
    public class SurveyQuestionBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Sequence { get; set; }
        public bool Active { get; set; }
    }

    public class SurveyQuestionDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Sequence { get; set; }
        public List<SurveyAnswerDisplay> Answers { get; set; } = new List<SurveyAnswerDisplay>();
    }

    public class SurveyQuestionSave
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public ICollection<SurveyAnswerSave> Answers { get; set; } = new List<SurveyAnswerSave>();
    }

    public class SurveyQuestionUpdateListPar
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Sequence { get; set; }
    }

    public class SurveyQuestionResequenceVM
    {
        public int Offset { get; set; }

        public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
    }

    public class SwapPar
    {
        public Guid IdFrom { get; set; }
        public Guid IdTo { get; set; }
    }

    public class ActionActivePar
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
    }
}
