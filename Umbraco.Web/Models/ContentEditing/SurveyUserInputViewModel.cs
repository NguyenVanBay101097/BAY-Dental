using ApplicationCore.Entities;
using System;
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


    }
    public class SurveyUserInputBasic
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Số điểm đạt được, compute
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// Số điểm tối đa, compute
        /// </summary>
        public decimal? MaxScore { get; set; }
    }

    public class SurveyUserInputDisplay
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Số điểm đạt được, compute
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// Số điểm tối đa, compute
        /// </summary>
        public decimal? MaxScore { get; set; }

        /// <summary>
        /// nội dung khảo sát
        /// </summary>
        public string Note { get; set; }

        public IEnumerable<SurveyUserInputLineDisplay> Lines { get; set; } = new List<SurveyUserInputLineDisplay>();
    }

    public class SurveyUserInputSave
    {

        /// <summary>
        /// Số điểm đạt được, compute
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// Số điểm tối đa, compute
        /// </summary>
        public decimal? MaxScore { get; set; }

        /// <summary>
        /// nội dung khảo sát
        /// </summary>
        public string Note { get; set; }

        public IEnumerable<SurveyUserInputLineSave> Lines { get; set; } = new List<SurveyUserInputLineSave>();
    }

    public class SurveyUserInputDefaultGet
    {
        public Guid? SurveyAssignmentId { get; set; }
    }
}
