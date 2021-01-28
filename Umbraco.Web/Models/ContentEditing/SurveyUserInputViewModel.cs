﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class SurveyUserInputPaged {
        public SurveyUserInputPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public string Type { get; set; }

    }
    public class SurveyUserInputBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Sequence { get; set; }
    }

    public class SurveyUserInputDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Sequence { get; set; }
        public IEnumerable<SurveyAnswerDisplay> Answers { get; set; } = new List<SurveyAnswerDisplay>();
    }

    public class SurveyUserInputSave
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Sequence { get; set; }
        public IEnumerable<SurveyAnswerDisplay> Answers { get; set; } = new List<SurveyAnswerDisplay>();
    }
}
