using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TenantOldSaleOrderProcessUpdateListVM
    {
        public Guid Id { get; set; }

        public string TenantHostName { get; set; }

        public string State { get; set; }
    }

    public class TenantOldSaleOrderProcessUpdateFilterPagedVM
    {
        public TenantOldSaleOrderProcessUpdateFilterPagedVM()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Search { get; set; }
    }
}
