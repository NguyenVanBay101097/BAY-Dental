using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AgentPaged
    {
        public AgentPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }
    
    }

    public class AgentBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class AgentSave
    {
        public string Name { get; set; }
        public string Gender { get; set; }

        /// <summary>
        /// Năm sinh
        /// </summary>
        public int? BirthYear { get; set; }

        /// <summary>
        /// Tháng sinh
        /// </summary>
        public int? BirthMonth { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public int? BirthDay { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }
    }

    public class AgentDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        /// <summary>
        /// Năm sinh
        /// </summary>
        public int? BirthYear { get; set; }

        /// <summary>
        /// Tháng sinh
        /// </summary>
        public int? BirthMonth { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public int? BirthDay { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        public string JobTitle { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }
    }

    public class CommissionAgentFilter
    {
        public CommissionAgentFilter()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Search { get; set; }
    }

    public class CommissionAgentResult
    {
       public AgentBasic Agent { get; set; }
       public decimal AmountTotal { get; set; }
       public decimal AmountCommissionTotal { get; set; }
    }
}
