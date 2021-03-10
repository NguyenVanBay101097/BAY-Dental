using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class SurveyCallContentPaged
    {
        public SurveyCallContentPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public Guid? AssignmentId { get; set; }
    }

    public class SurveyCallContentBasic
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }

    public class SurveyCallContentDisplay
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }

    public class SurveyCallContentSave
    {
        public Guid? AssignmentId { get; set; }
        public string Name { get; set; }
    }
}
