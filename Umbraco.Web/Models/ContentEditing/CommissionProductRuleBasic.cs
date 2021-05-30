using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CommissionProductRuleBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// % hoa hồng 
        /// </summary>
        public decimal? Percent { get; set; }      

        public string AppliedOn { get; set; }
    }
}
