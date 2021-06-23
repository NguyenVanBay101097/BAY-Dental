using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerInfoDisplay
    {
        public Guid Id { get; set; }
        public DateTime? DateCreated { get; set; }

        public string Ref { get; set; }
        public string Avatar { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }
        public string OrderState { get; set; }
        public decimal? OrderResidual { get; set; }
        public decimal? TotalDebit { get; set; }
        public Guid? MemberLevelId { get; set; }
        public MemberLevelBasic MemberLevel { get; set; }
        public IEnumerable<PartnerCategoryBasic> Categories { get; set; } = new List<PartnerCategoryBasic>();
        public string DateOfBirth
        {
            get
            {
                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "_")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "_")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "_")}";
            }
            set { }
        }

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

    public class PartnerInfoPaged
    {
        public PartnerInfoPaged()
        {
            Offset = 0;
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public IEnumerable<Guid> CategIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 0: không có doanh thu, 1: có doanh thu
        /// </summary>
        public int? HasOrderResidual { get; set; }
        /// <summary>
        /// 0: không có , 1: có 
        /// </summary>
        public int? HasTotalDebit { get; set; }
        public Guid? MemberLevelId { get; set; }
        public string OrderState { get; set; }

    }
}
