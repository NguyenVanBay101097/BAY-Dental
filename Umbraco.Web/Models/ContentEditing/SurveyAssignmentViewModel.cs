using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SurveyAssignmentPaged
    {
        public SurveyAssignmentPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public string Status { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public bool? IsGetScore { get; set; }
        public Guid? EmployeeId { get; set; }

    }

    public class SurveyAssignmentBasic
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public EmployeeSimple Employee { get; set; }
        public Guid SaleOrderId { get; set; }
        public SaleOrderSurveyBasic SaleOrder { get; set; }
        public string Status { get; set; }
        public DateTime? CompleteDate { get; set; }
        public decimal? UserInputScore { get; set; }
        public decimal? UserInputMaxScore { get; set; }
        public string PartnerName { get; set; }
        public string PartnerRef { get; set; }
        public Guid PartnerId { get; set; }
        public string PartnerPhone { get; set; }

        public string PartnerGender { get; set; }
        public string PartnerGenderDisplay
        {
            get
            {
                switch (this.PartnerGender)
                {
                    case "female": return "Nữ";
                    case "male": return "Nam";
                    default: return "khác";
                }
            }
            set { }
        }

        public string Age
        {
            get
            {
                if (!PartnerBirthYear.HasValue)
                {
                    return string.Empty;
                }

                return (DateTime.Now.Year - PartnerBirthYear.Value).ToString();
            }
            set
            {
            }
        }

        public int? PartnerBirthYear { get; set; }

        public string PartnerCategoriesDisplay { get; set; }
    }

    public class SurveyAssignmentDisplay
    {
        public Guid Id { get; set; }

        /// <summary>
        /// detail
        /// </summary>
        public Guid SaleOrderId { get; set; }
        public SaleOrderDisplay SaleOrder {get;set;}

        public Guid SurveyUserInputId { get; set; }
        public SurveyUserInputDisplay SurveyUserInput { get; set; }

        public IEnumerable<SurveyCallContentDisplay> CallContents { get; set; } = new List<SurveyCallContentDisplay>();

        public string Status { get; set; }
    }

    public class SurveyAssignmentSave
    {
        public SurveyAssignmentSave()
        {
            Status = "draft";
        }

        public Guid EmployeeId { get; set; }
        public Guid SaleOrderId { get; set; }

        public IEnumerable<SurveyCallContentSave> CallContents { get; set; } = new List<SurveyCallContentSave>();

        public string Status { get; set; }
        public Guid PartnerId { get; set; }
    }

    public class SurveyAssignmentDefaultGet
    {
        public SurveyAssignmentDefaultGet()
        {
            Status = "draft";
        }
        public Guid SaleOrderId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerRef { get; set; }
        public string PartnerId { get; set; }
        public string PartnerPhone { get; set; }
        public string SaleOrderName { get; set; }
        public DateTime DateOrder { get; set; }
        public string Status { get; set; }
    }



    public class SurveyAssignmentPatch
    {
        public Guid EmployeeId { get; set; }
    }


}
