using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerSourcePaged
    {
        public PartnerSourcePaged()
        {
            Limit = 10;           
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        /// <summary>
        /// normal : nguồn facebook , zalo , news v.v..
        /// referral : người giới thiệu
        /// </summary>
        public string Type { get; set; }
        public string Search { get; set; }

    }
}
