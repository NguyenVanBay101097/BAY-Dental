using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AdvisoryBasic
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public EmployeeSimple Employee { get; set; }
        public string ToothType { get; set; }
        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();
        public IEnumerable<ToothDiagnosisBasic> ToothDiagnosis { get; set; } = new List<ToothDiagnosisBasic>();
        public IEnumerable<ProductSimple> Product { get; set; } = new List<ProductSimple>();
    }

    public class AdvisoryPaged
    {
        public AdvisoryPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CustomerId { get; set; }
        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();
        public Guid? CompanyId { get; set; }
    }

    public class AdvisorySave
    {
        public Guid CustomerId { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public Guid? ToothCategoryId { get; set; }
        /// <summary>
        /// whole_jaw: nguyên hàm
        /// upper_jaw : hàm trên
        /// lower_jaw : hàm dưới
        /// manual :  thủ công
        /// </summary>
        public string ToothType { get; set; }
        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();
        public IEnumerable<Guid> ToothDiagnosisIds { get; set; } = new List<Guid>();
        public IEnumerable<Guid> ProductIds { get; set; } = new List<Guid>();
        public string Note { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class AdvisoryDisplay
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }
        public PartnerSimple Customer { get; set; }
        /// <summary>
        /// Người tư vấn
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }
        public DateTime Date { get; set; }
        public Guid? ToothCategoryId { get; set; }
        public ToothCategoryBasic ToothCategory { get; set; }
        /// <summary>
        /// whole_jaw: nguyên hàm
        /// upper_jaw : hàm trên
        /// lower_jaw : hàm dưới
        /// manual :  thủ công
        /// </summary>
        public string ToothType { get; set; }
        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();
        public IEnumerable<ToothDiagnosisBasic> ToothDiagnosis { get; set; } = new List<ToothDiagnosisBasic>();
        public IEnumerable<ProductSimple> Product { get; set; } = new List<ProductSimple>();
        public string Note { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class AdvisoryLine
    {
        public DateTime? Date { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public string DoctorName { get; set; }
        public decimal Qty { get; set; }
        public string Type { get; set; }
    }

    public class AdvisoryLinePaged
    {
        public AdvisoryLinePaged()
        {
            Limit = 10;
        }

        public int Offset { get; set; }
        public int Limit { get; set; }
        public Guid AdvisoryId { get; set; }
    }


    public class AdvisoryDefaultGet
    {
        public Guid? CustomerId { get; set; }
    }

    public class AdvisoryToothAdvise
    {
        public Guid? CustomerId { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class ToothAdvised
    {
        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();
    }

    public class AdvisoryPrintVM
    {
        public CompanyPrintVM Company { get; set; }
        public PartnerInfoVm Partner { get; set; }
        public IEnumerable<AdvisoryItemPrintVM> Advisories { get; set; }
    }

    public class AdvisoryItemPrintVM
    {
        //Ngày tư vấn
        public DateTime Date { get; set; }

        /// <summary>
        /// Người tư vấn
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public EmployeeSimple Employee { get; set; }

        public string ToothType { get; set; }

        /// <summary>
        /// ToothType Or list name tooth string join
        /// </summary>
        public string Tooths { get; set; }



        /// <summary>
        /// list name Diagnosis string join
        /// </summary>
        public string Diagnosis { get; set; }

        /// <summary>
        /// list name services
        /// </summary>
        public string Services { get; set; }

    }

    public class CreateFromAdvisoryInput
    {
        public Guid CustomerId { get; set; }
        public IEnumerable<Guid> Ids { get; set; } = new List<Guid>();
    }

}
