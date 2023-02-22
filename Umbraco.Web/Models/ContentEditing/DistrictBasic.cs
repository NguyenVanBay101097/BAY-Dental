using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DistrictBasic: DistrictSimple
    {
        public Guid ProvinceId { get; set; }
        public ProvinceSimple Province { get; set; }
    }
}
