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
        public string PartnerCategIds { get; set; }
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
        public string Search { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public Guid? CategId { get; set; }
        public bool? IsResidual { get; set; }
        public bool? IsTotalDebit { get; set; }
        public Guid? MemberLevelId { get; set; }
        public string State { get; set; }

    }
}
