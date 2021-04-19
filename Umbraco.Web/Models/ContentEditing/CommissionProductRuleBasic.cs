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
        /// % hoa hồng tư vấn
        /// </summary>
        public decimal? PercentAdvisory { get; set; }

        /// <summary>
        /// % hoa hồng bác sĩ
        /// </summary>
        public decimal? PercentDoctor { get; set; }

        /// <summary>
        /// % hoa hồng assistant
        /// </summary>
        public decimal? PercentAssistant { get; set; }

        public string AppliedOn { get; set; }
    }
}
