using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AddressCheckApi
    {
        public string Address { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string ShortAddress { get; set; }
        public string Telephone { get; set; }
        public string WardCode { get; set; }
        public string WardName { get; set; }
    }
}
