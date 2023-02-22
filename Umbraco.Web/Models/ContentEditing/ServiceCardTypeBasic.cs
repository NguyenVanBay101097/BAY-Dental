using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardTypeBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Period { get; set; }

        public int? NbrPeriod { get; set; }

        public Guid? ProductPricelistId { get; set; }
    }
}
