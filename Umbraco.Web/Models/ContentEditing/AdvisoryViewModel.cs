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
        public ApplicationUserSimple User { get; set; }
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
        public IEnumerable<Guid> ToothIds { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class AdvisorySave
    {
        public Guid CustomerId { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public Guid? ToothCategoryId { get; set; }
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
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }
        public DateTime Date { get; set; }
        public Guid? ToothCategoryId { get; set; }
        public ToothCategoryBasic ToothCategory { get; set; }
        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();
        public IEnumerable<ToothDiagnosisBasic> ToothDiagnosis { get; set; } = new List<ToothDiagnosisBasic>();
        public IEnumerable<ProductSimple> Product { get; set; } = new List<ProductSimple>();
        public string Note { get; set; }
        public Guid? CompanyId { get; set; }
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
}
