using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CongEmplyee
    {
        public CongEmplyee()
        {
            SoCong = 0;
            CongChuan1Thang = 0;
        }
        public decimal? SoCong { get; set; }
        public int CongChuan1Thang { get; set; }
    }
}
