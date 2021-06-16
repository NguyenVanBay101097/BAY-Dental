using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class VPartnerInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ref { get; set; }
        public string Phone { get; set; }
        public int? BirthDay { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthYear { get; set; }
        public decimal? Residual { get; set; }
        public decimal? TotalDebit { get; set; }
        public string State { get; set; }
        public double? MemberLevel { get; set; }
        public string PartnerCategIds { get; set; }
    }
}
