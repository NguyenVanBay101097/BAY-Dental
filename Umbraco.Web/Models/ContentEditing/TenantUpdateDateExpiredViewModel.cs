using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TenantUpdateDateExpiredViewModel
    {
        public Guid Id { get; set; }
        public DateTime? DateExpired { get; set; }
        public int ActiveCompaniesNbr { get; set; }
    }

    public class TenantExtendExpiredViewModel
    {
        public int ActiveCompaniesNbr { get; set; }
        public Guid TenantId { get; set; }
        public int Limit { get; set; }
        public string CheckOption { get; set; }
        public string LimitOption { get; set; }
    }
}
