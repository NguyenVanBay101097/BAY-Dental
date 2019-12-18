using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ResConfigSettingsModel
    {
        public Guid CompanyId { get; set; }

        [DbColumn("implied_group", "sale.group_discount_per_so_line")]
        [DbColumn("field_type", "boolean")]
        public bool? GroupDiscountPerSOLine { get; set; }
    }
}
