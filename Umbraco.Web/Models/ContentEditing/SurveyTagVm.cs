using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class SurveyTagPaged
    {
        public SurveyTagPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
    }

    public class SurveyTagBasic
    {
        public Guid Id { get; set; }
        public string  Name { get; set; }
        public string Color { get; set; }
    }

    public class SurveyTagSave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
