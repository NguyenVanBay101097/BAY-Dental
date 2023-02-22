using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderLineOnChangeCardType
    {
        public Guid? CardTypeId { get; set; }
    }

    public class ServiceCardOrderLineOnChangeCardTypeResponse
    {
        public decimal PriceUnit { get; set; }
    }
}
