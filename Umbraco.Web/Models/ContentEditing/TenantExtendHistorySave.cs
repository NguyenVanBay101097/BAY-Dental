using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TenantExtendHistorySave
    {
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int ActiveCompaniesNbr { get; set; }
        public Guid? TenantId { get; set; }
    }

    public class TenantExtendHistoryDisplay
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ActiveCompaniesNbr { get; set; }
        public Guid TenantId { get; set; }
        public TenantBasic AppTenant { get; set; }
    }
}
