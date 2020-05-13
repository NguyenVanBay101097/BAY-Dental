using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string PartnerName { get; set; }

        /// <summary>
        /// Ngày bán
        /// </summary>
        public DateTime DateOrder { get; set; }

        public string UserName { get; set; }

        public string State { get; set; }

        public decimal? AmountTotal { get; set; }

        public decimal? AmountResidual { get; set; }
    }
}
