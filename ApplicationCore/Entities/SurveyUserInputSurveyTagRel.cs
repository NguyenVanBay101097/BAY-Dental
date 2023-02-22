using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SurveyUserInputSurveyTagRel
    {
        public Guid UserInputId { get; set; }
        public SurveyUserInput UserInput { get; set; }

        public Guid SurveyTagId { get; set; }
        public SurveyTag SurveyTag { get; set; }
    }
}
