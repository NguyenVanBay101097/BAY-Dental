using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerReportLocationCity
    {
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public int Total { get; set; }
        public float Percentage { get; set; }

        public string SearchCityCode { get; set; }
        public string SearchDistrictCode { get; set; }
        public string SearchWardCode { get; set; }
    }

    public class PartnerReportLocationItem
    {
        public string Name { get; set; }
        public int Total { get; set; }
        public float Percentage { get; set; }
    }

    public class PartnerReportLocationDistrict
    {
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int Total { get; set; }
        public float Percentage { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }

        public string SearchWardCode { get; set; }
    }

    public class PartnerReportLocationWard
    {
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public int Total { get; set; }
        public float Percentage { get; set; }
    }
}
