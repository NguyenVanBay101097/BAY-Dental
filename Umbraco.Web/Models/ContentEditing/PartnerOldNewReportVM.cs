using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerOldNewReportVM
    {
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public int TotalNewPartner { get; set; }
        public int TotalOldPartner { get; set; }
        public IEnumerable<PartnerOldNewReportVMDetail> OrderLines { get; set; } = new List<PartnerOldNewReportVMDetail>();
    }

    public class PartnerOldNewReportVMDetail
    {
        public Guid PartnerId { get; set; }
        public DateTime Date { get; set; }
        public string PartnerName { get; set; }
        public string OrderName { get; set; }
        public int CountLine { get; set; }
        public string Type { get; set; }
    }

    public class PartnerOldNewReportSearch
    {
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        public DateTime? DateTo { get; set; }

    }
    public class PartnerOldNewReportSumReq
    {
        public PartnerOldNewReportSumReq() { }
        public PartnerOldNewReportSumReq(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string TypeReport = "")
        {
            this.DateFrom = dateFrom;
            this.DateTo = DateTo;
            this.CompanyId = companyId;
            this.TypeReport = TypeReport;
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// old, new
        /// </summary>
        public String TypeReport { get; set; }
    }
    public class PartnerOldNewReportReq
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// old, new
        /// </summary>
        public String TypeReport { get; set; }
        //code city, quận  phường
        public string CityCode { get; set; }
        public string DistrictCode { get; set; }
        public string WardCode { get; set; }
        // nguồn khách hàng
        public Guid? SourceId { get; set; }
        //nhãn
        public IEnumerable<Guid> CategIds { get; set; } = new List<Guid>();
        //hạng thành viên
        public Guid? MemberLevelId { get; set; }
        //giới tính
        public string Gender { get; set; }
        public string Search { get; set; }
    }

    public class PartnerOldNewReportRes
    {
        public Guid Id { get; set; }
        public string Ref { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int? BirthYear { get; set; }
        public string OrderState { get; set; }
        public decimal Revenue { get; set; }
        public MemberLevelBasic MemberLevel { get; set; }
        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();
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


}
