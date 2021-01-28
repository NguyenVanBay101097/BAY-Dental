using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderViewModel
    {
        public Guid Id { get; set; }

        public DateTime DateOrder { get; set; }

        public Guid PartnerId { get; set; }

        public string PartnerDisplayName { get; set; }

        public decimal? AmountTotal { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public decimal? Residual { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Là phiếu tư vấn
        /// </summary>
        public bool? IsQuotation { get; set; }
    }

    public class SaleOrderSurveyBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOrder { get; set; }
        public DateTime LastUpdated { get; set; }
        public string PartnerName { get; set; }
        public string PartnerRef { get; set; }
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

        public IEnumerable<PartnerCategoryBasic> PartnerCategories { get; set; } = new List<PartnerCategoryBasic>();
        public string PartnerCategoriesDisplay
        {
            get
            {
                return string.Join(" ,", PartnerCategories.Select(x=> x.Name));
            }
            set { }
        }
    }

    public class ActionDonePar
    {
        public IEnumerable<Guid> Ids { get; set; }
    }
}
