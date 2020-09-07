using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ChamCongPaged
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

    }
}
