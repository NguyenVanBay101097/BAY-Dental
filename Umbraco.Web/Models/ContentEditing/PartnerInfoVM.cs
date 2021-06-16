﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerInfoVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ref { get; set; }
        public string Phone { get; set; }
        public int? BirthDay { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthYear { get; set; }
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
        public decimal? Residual { get; set; }
        public decimal? TotalDebit { get; set; }
        public double? MemberLevel { get; set; }
        public string PartnerCategIds { get; set; }
    }

    public class PartnerViewInfoPaged
    {
        public PartnerViewInfoPaged()
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
