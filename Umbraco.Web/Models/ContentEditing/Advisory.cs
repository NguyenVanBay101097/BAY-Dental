using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AdvisoryBasic
    {
        public AdvisoryBasic()
        {
            Date = DateTime.Now;
        }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();
        public IEnumerable<ToothDiagnosisBasic> ToothDiagnosis { get; set; } = new List<ToothDiagnosisBasic>();
        public IEnumerable<ProductBasic> Product { get; set; } = new List<ProductBasic>();
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
    }

    public class AdvisorySave
    {
        public AdvisorySave()
        {
            Date = DateTime.Now;
        }
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
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();
        public IEnumerable<ToothDiagnosisBasic> ToothDiagnosis { get; set; } = new List<ToothDiagnosisBasic>();
        public IEnumerable<ProductBasic> Product { get; set; } = new List<ProductBasic>();
        /// <summary>
        /// ghi chú
        /// </summary>
        public string Note { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class AdvisoryDisplay
    {
        public AdvisoryDisplay()
        {
            Date = DateTime.Now;
        }
        public Guid Id { get; set; }
        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }
        public PartnerSimple Customer { get; set; }
        /// <summary>
        /// Người tư vấn
        /// </summary>
        public Guid UserId { get; set; }
        public ApplicationUserSimple User { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();
        public IEnumerable<ToothDiagnosisBasic> ToothDiagnosis { get; set; } = new List<ToothDiagnosisBasic>();
        public IEnumerable<ProductBasic> Product { get; set; } = new List<ProductBasic>();
        /// <summary>
        /// ghi chú
        /// </summary>
        public string Note { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class AdvisoryDefaultGet
    {
        public Guid CustomerId { get; set; }
    }
}
