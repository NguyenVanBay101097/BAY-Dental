using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class WardBasic: WardSimple
    {
        public Guid DistrictId { get; set; }
        public DistrictSimple District { get; set; }
    }
}
