using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderOnChangePartner
    {
        public Guid? PartnerId { get; set; }
    }
    public class SaleOrderOnChangePartnerResult
    {
        public ProductPricelistBasic Pricelist { get; set; }
    }
}
