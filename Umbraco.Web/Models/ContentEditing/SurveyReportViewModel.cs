using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class GetReportAssignmentQueryRequest
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public string Status { get; set; }
    }
    public class ReportNumberOfAssigmentByEmployeeRequest : GetReportAssignmentQueryRequest
    {
    }

    public class ReportRatingScroreRateOfUserInputRequest : GetReportAssignmentQueryRequest
    {
    }

    public class ReportSatifyScoreRatingByQuestionRequest : GetReportAssignmentQueryRequest
    {

    }

    public class ReportNumberOfAssigmentByEmployeeResponse
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int DoneCount { get; set; }
        public int ContactCount { get; set; }
    }

    public class ReportRatingScroreRateOfUserInputResponse
    {
        public IEnumerable<ReportRatingScroreRateOfUserInputResponseItem> Lines = new List<ReportRatingScroreRateOfUserInputResponseItem>();
        public decimal MaxScore { get; set; }
        public decimal Score { get; set; }
    }

    public class ReportRatingScroreRateOfUserInputResponseItem
    {
        public string Name
        {
            get
            {
                return String.Format("{0} - {1} điểm", this.ScroreFrom, this.ScroreTo);
            }
        }
        public decimal ScroreFrom { get; set; }
        public decimal ScroreTo { get; set; }
        public int Value { get; set; }
    }

    public class ReportSatifyScoreRatingByQuestionResponseItem
    {
        public decimal Score { get; set; }
        public IEnumerable<int> Data { get; set; } = new List<int>();
    }

    public class ReportSatifyScoreRatingByQuestionResponse
    {
        public IEnumerable<string> QuestionNames { get; set; } = new List<string>();
        public IEnumerable<ReportSatifyScoreRatingByQuestionResponseItem> Data { get; set; } = new List<ReportSatifyScoreRatingByQuestionResponseItem>();
    }
}
