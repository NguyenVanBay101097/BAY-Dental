using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TenantOldSaleOrderProcessUpdate: AdminBaseEntity
    {
        public TenantOldSaleOrderProcessUpdate()
        {
            State = "draft";
        }

        public Guid TenantId { get; set; }
        public AppTenant Tenant { get; set; }

        /// <summary>
        /// draft: Mới
        /// done: Hoàn thành
        /// exception: Exception
        /// </summary>
        public string State { get; set; }
    }
}
