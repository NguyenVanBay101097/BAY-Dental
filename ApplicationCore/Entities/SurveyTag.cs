using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SurveyTag : BaseEntity
    {
        public string Name { get; set; }

        public string Color { get; set; }

        public ICollection<SurveyUserInputSurveyTagRel> SurveyUserInputSurveyTagRels { get; set; } = new List<SurveyUserInputSurveyTagRel>();
    }
}
