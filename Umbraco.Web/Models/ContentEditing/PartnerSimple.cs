using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

    }

    public class PartnerSimpleContact
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class PartnerSimpleInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Ref { get; set; }

        public string Gender { get; set; }

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

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        public int? BirthYear { get; set; }

        /// <summary>
        /// Công nợ
        /// </summary>
        public decimal Debt { get; set; }

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

        public string CompanyName { get; set; }
        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();
    }

}
