using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.PartnerEndpoints.GetCustomerInfo
{
    public class GetCustomerInfoPartnerResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Phone { get; set; }

        public DateTime? Date { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string Ref { get; set; }

        public string Gender { get; set; }
        public string GenderConvertcom
        {
            get
            {
                switch (this.Gender)
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

        public string HistoriesString
        {
            get
            {
                var list = new List<string>();
                foreach (var item in this.Histories)
                {
                    list.Add(item.Name);
                }
                return string.Join(", ", list);
            }
            set { }
        }

        public string Tags
        {
            get
            {
                var list = new List<string>();
                foreach (var item in this.Categories)
                {
                    list.Add(item.Name);
                }
                return string.Join(", ", list);
            }
            set { }
        }

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
        /// Tiền sử bệnh, nên hiển thị nếu là bệnh nhân (khách hàng)
        /// </summary>
        public string MedicalHistory { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Ảnh chân dung
        /// </summary>
        public string Avatar { get; set; }

        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();

        public IEnumerable<HistorySimple> Histories { get; set; } = new List<HistorySimple>();

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

        public decimal? Point { get; set; }

        public MemberLevelBasic MemberLevel { get; set; }
    }
}
