using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SearchAllViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string SaleOrderName { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string Note { get; set; }
        public List<PartnerCategoryBasic> Tags { get; set; } = new List<PartnerCategoryBasic>();
    }
}
