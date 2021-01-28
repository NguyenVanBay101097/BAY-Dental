using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
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
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
}
