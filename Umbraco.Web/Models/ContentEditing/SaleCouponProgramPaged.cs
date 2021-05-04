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


        public bool? Active { get; set; }

        public IEnumerable<Guid> Ids { get; set; }
    }

    public class SaleCouponProgramResponse
    {
        public bool Success { get; set; }
        public SaleCouponProgramDisplay SaleCouponProgram { get; set; }
        public string Error { get; set; }
    }
}
