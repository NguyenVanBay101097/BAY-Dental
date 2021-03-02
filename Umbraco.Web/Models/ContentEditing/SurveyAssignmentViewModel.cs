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
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? UserId { get; set; }
    }

    public class SurveyAssignmentGridItem
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public EmployeeSimple Employee { get; set; }
        public Guid EmployeeId { get; set; }
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
        public DateTime? AssignDate { get; set; }
    }

    public class SurveyAssignmentDisplay
    {
        public Guid Id { get; set; }

        public SurveyAssignmentDisplayPartner Partner { get; set; }

        public SurveyAssignmentDisplaySaleOrder SaleOrder { get; set; }

        public IEnumerable<SurveyAssignmentDisplaySaleOrderLine> SaleLines { get; set; }

        public IEnumerable<SurveyAssignmentDisplayDotKham> DotKhams { get; set; }

        public IEnumerable<SurveyAssignmentDisplayCallContent> CallContents { get; set; }

        public Guid? UserInputId { get; set; }
        public SurveyUserInputDisplay UserInput { get; set; }

        public string Status { get; set; }
    }

    public class SurveyAssignmentDisplayPartner
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Ref { get; set; }

        public string Gender { get; set; }
        public string GenderDisplay
        {
            get
            {
                switch (Gender)
                {
                    case "female": return "Nữ";
                    case "male": return "Nam";
                    default: return "khác";
                }
            }
            set { }
        }

        public string DateOfBirth
        {
            get
            {
                if (!BirthDay.HasValue && !BirthMonth.HasValue && !BirthYear.HasValue)
                    return string.Empty;

                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "--")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "--")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "----")}";
            }
            set { }
        }

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public string Address
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(Street))
                    list.Add(Street);
                if (!string.IsNullOrEmpty(WardName))
                    list.Add(WardName);
                if (!string.IsNullOrEmpty(DistrictName))
                    list.Add(DistrictName);
                if (!string.IsNullOrEmpty(CityName))
                    list.Add(CityName);
                return string.Join(", ", list);
            }
            set { }
        }

        public IEnumerable<string> Histories { get; set; }

        public int? BirthYear { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Tháng sinh
        /// </summary>
        public int? BirthMonth { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public int? BirthDay { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Ảnh chân dung
        /// </summary>
        public string Avatar { get; set; }

        public IEnumerable<string> Categories { get; set; }

        public DateTime? Date { get; set; }

        public string Age
        {
            get
            {
                if (!BirthYear.HasValue)
                {
                    return string.Empty;
                }

                return (DateTime.Now.Year - BirthYear.Value).ToString();
            }
            set
            {
            }
        }
    }

    public class SurveyAssignmentDisplaySaleOrder
    {
        public string Name { get; set; }

        public DateTime DateOrder { get; set; }

        public string State { get; set; }

        public string StateDisplay
        {
            get
            {
                switch (State)
                {
                    case "done":
                        return "Hoàn thành";
                    case "sale":
                        return "Đang điều trị";
                    default:
                        return "Nháp";
                }
            }
            set { }
        }

        public decimal? AmountTotal { get; set; }
    }

    public class SurveyAssignmentDisplaySaleOrderLine
    {
        public string ProductName { get; set; }

        public string EmployeeName { get; set; }

        public decimal ProductUOMQty { get; set; }

        public IEnumerable<string> Teeth { get; set; }

        public string Diagnostic { get; set; }
    }

    public class SurveyAssignmentDisplayDotKham
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Reason { get; set; }

        public string DoctorName { get; set; }

        public IEnumerable<SurveyAssignmentDisplayDotKhamLine> Lines { get; set; } = new List<SurveyAssignmentDisplayDotKhamLine>();
    }

    public class SurveyAssignmentDisplayDotKhamLine
    {
        public string NameStep { get; set; }

        public string ProductName { get; set; }

        public string Note { get; set; }

        public IEnumerable<string> Teeth { get; set; } = new List<string>();
    }

    public class SurveyAssignmentDisplayCallContent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class SurveyAssignmentSave
    {
        public Guid EmployeeId { get; set; }
        public Guid SaleOrderId { get; set; }
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
        public Guid PartnerId { get; set; }
        public string PartnerPhone { get; set; }
        public string SaleOrderName { get; set; }
        public DateTime DateOrder { get; set; }
        public EmployeeSimple Employee { get; set; }
        public Guid? EmployeeId { get; set; }
        public string Status { get; set; }
        public DateTime? SaleOrderDateCreated { get; set; }
        public DateTime? SaleOrderDateDone { get; set; }

    }

    public class SurveyAssignmentDefaultGetPar
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
    }

    public class AssignmentActionDone
    {
        public Guid Id { get; set; }
        public SurveyUserInputSave SurveyUserInput { get; set; }
    }

    public class SurveyAssignmentPatch
    {
        public Guid EmployeeId { get; set; }
    }

    public class SurveyAssignmentGetSummaryFilter
    {
        public string Status { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public string UserId { get; set; }
    }

    public class SurveyAssignmentGetSummary
    {
        public string Status { get; set; }

        public int Count { get; set; }
    }

    public class SurveyAssignmentUpdateEmployee
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
    }

    public class EmployeeCountSurvey
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

}
