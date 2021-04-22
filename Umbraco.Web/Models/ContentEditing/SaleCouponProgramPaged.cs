using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleCouponProgramPaged
    {
        public SaleCouponProgramPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public string ProgramType { get; set; }

        /// <summary>
        /// on_order : trên phiếu điều trị
        /// specific_products :  trên dịch vụ
        /// </summary>
        public string DiscountApplyOn { get; set; }

        /// <summary>
        /// code_needed : khuyen mai su dung code
        /// no_code_needed : khuyen mai ko su dung code
        /// </summary>
        public string PromoCodeUsage { get; set; }


        public bool? Active { get; set; }

        public IEnumerable<Guid> Ids { get; set; }
    }
}
