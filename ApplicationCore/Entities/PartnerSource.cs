using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PartnerSource : BaseEntity
    {
        public PartnerSource()
        {
            Type = "normal";
        }

        public string Name { get; set; }

        /// <summary>
        /// normal : nguồn facebook , zalo , news v.v..
        /// referral : người giới thiệu
        /// </summary>
        public string Type { get; set; }
    }
}
